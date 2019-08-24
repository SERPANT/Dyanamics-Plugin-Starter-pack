using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

using OrganizationName.PluginRegistration.Entities;

namespace OrganizationName.PluginRegistration.Services
{
    public class PluginRegistrationService
    {
        private OrganizationName.PluginRegistration.Entities.IUnitOfWork uow { get; set; }

        public PluginRegistrationService(OrganizationName.PluginRegistration.Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public void Registre(string assemblyFilePath)
        {
            var code = System.IO.File.ReadAllBytes(assemblyFilePath);

            var assm = System.Reflection.Assembly.Load(code);

            var publickeytoken = PluginRegistrationService.GetPublicKeyTokenFromAssembly(assm);

            //FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assm.Location);
            //var tmp = fvi.FileVersion.Split('.');
            //var version = tmp[0] + "." + tmp[1];

            var file = new System.IO.FileInfo(assemblyFilePath);
            var name = file.Name.Substring(0, file.Name.Length - 4);
            var ns = name + ".Attributes.";

            var solutionAttributeType = assm.GetType(ns + "SolutionAttribute");
            var stepAttributeType = assm.GetType(ns + "StepAttribute");


            var pluginType = typeof(Microsoft.Xrm.Sdk.IPlugin);
            var classes = (from cl in assm.GetTypes() where pluginType.IsAssignableFrom(cl) && !cl.IsAbstract select cl).ToArray();
            var steps = new List<Model.Step>();

            var plugin = (from pl in uow.PluginAssemblies.GetQuery()
                          where pl.Name == name
                          select pl).SingleOrDefault();

            if (plugin == null)
            {
                plugin = new OrganizationName.PluginRegistration.Entities.PluginAssembly
                {
                    PluginAssemblyId = Guid.NewGuid(),
                    Content = System.Convert.ToBase64String(code),
                    Description = name,
                    IsolationMode = new Microsoft.Xrm.Sdk.OptionSetValue(2),
                    Name = name,
                    SourceType = new Microsoft.Xrm.Sdk.OptionSetValue(0),
                    Culture = "neutral",
                    PublicKeyToken = publickeytoken,
                    Version = "1.0"
                };
                uow.Create(plugin);
            }

            var pluginTypes = (from pl in uow.PluginTypes.GetQuery()
                               where pl.PluginAssemblyId.Id == plugin.PluginAssemblyId
                               select pl).ToArray().ToDictionary(t => t.TypeName);

            var stepindex = (from st in uow.SdkMessageProcessingSteps.GetQuery()
                             join pt in uow.PluginTypes.GetQuery() on st.EventHandler.Id equals pt.PluginTypeId
                             where pt.PluginAssemblyId.Id == plugin.PluginAssemblyId
                             select st).ToArray().ToDictionary(s => s.UniqueName);

            foreach (var cl in classes)
            {
                var solution = cl.GetCustomAttributes(solutionAttributeType, true).SingleOrDefault();
                var assmSteps = cl.GetCustomAttributes(stepAttributeType, false).ToArray();

                if (solution != null && assmSteps.Length > 0)
                {
                    foreach (var assStep in assmSteps)
                    {

                        string[] eventTypes = ((IEnumerable)stepAttributeType.GetProperty("EventType").GetValue(assStep)).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

                        bool[] preImages;
                        bool[] postImages;

                        try
                        {
                            preImages = ((IEnumerable)stepAttributeType.GetProperty("Preimage").GetValue(assStep)).Cast<bool>()
                                .Select(image => image).ToArray();
                        }
                        catch
                        {
                            preImages = null;
                        }

                        try
                        {
                            postImages = ((IEnumerable)stepAttributeType.GetProperty("Postimage").GetValue(assStep)).Cast<bool>()
                                .Select(image => image).ToArray();
                        }
                        catch
                        {
                            postImages = null;
                        }

                        for (int index = 0 ; index < eventTypes.Length; index++)
                        {
                            var eventType = eventTypes[index];
                            var preImage = preImages != null ? preImages[index] : false;
                            var postImage = postImages != null ? postImages[index] : false;

                            var step = new Model.Step
                            {
                                Class = cl,
                                EventType = (Model.CrmEventType)Enum.Parse(typeof(Model.CrmEventType), eventType),
                                ExecutionOrder = (int)stepAttributeType.GetProperty("ExecutionOrder").GetValue(assStep),
                                FilteringAttributes = (string[])stepAttributeType.GetProperty("FilteringAttributes").GetValue(assStep),
                                Preimage = preImage,
                                Postimage = postImage,
                                PreimageAttributes = (string[])stepAttributeType.GetProperty("PreimageAttributes").GetValue(assStep),
                                PrimaryEntity = (string)stepAttributeType.GetProperty("PrimaryEntity").GetValue(assStep),
                                SecondaryEntity = (string)stepAttributeType.GetProperty("SecondaryEntity").GetValue(assStep),
                                Solution = new Model.Solution { Name = (string)solutionAttributeType.GetProperty("Name").GetValue(solution) },
                                Stage = (Model.StageEnum)Enum.Parse(typeof(Model.StageEnum), stepAttributeType.GetProperty("Stage").GetValue(assStep).ToString()),
                                Offline = (bool)stepAttributeType.GetProperty("Offline").GetValue(assStep)
                            };

                            if (step.ExecutionOrder == 0)
                            {
                                step.ExecutionOrder = 1;
                            }
                            steps.Add(step);
                        }
                    }

                }
                else
                {
                    Console.WriteLine(cl.FullName + " is missing step or solution");
                }
            }

            #region find steps no longer in the solution and delete them
            {
                var dubs = (from s in steps
                            group s by s.UniqueName into grp
                            select new
                            {
                                Name = grp.Key,
                                Count = grp.Count()
                            }).Where(r => r.Count > 1).ToArray();
                if (dubs.Length > 0)
                {
                    foreach (var dub in dubs)
                    {
                        Console.WriteLine("Dublicate workflow: " + dub.Name);
                    }
                    return;
                }

                var newstepindex = steps.ToDictionary(s => s.UniqueName);
                var gones = (from s in stepindex.Values where !newstepindex.ContainsKey(s.UniqueName) select s).ToArray();
                foreach (var gon in gones)
                {
                    Console.WriteLine("Delete step: " + gon.Name);
                    uow.Delete(gon);
                    stepindex.Remove(gon.UniqueName);
                }
            }
            #endregion

            #region remove plugin types no longer represented in the assembly
            {
                var newtypeindex = (from s in steps select s.Class.FullName).Distinct().ToArray();
                var gones = (from p in pluginTypes.Values where !newtypeindex.Contains(p.TypeName) select p).ToArray();
                foreach (var gon in gones)
                {
                    Console.WriteLine("Delete plugin: " + gon.TypeName);
                    uow.Delete(gon);
                    pluginTypes.Remove(gon.TypeName);
                }
            }
            #endregion

            #region upload new assembly
            {
                var clean = uow.PluginAssemblies.Clean(plugin);
                clean.Content = System.Convert.ToBase64String(code);
                uow.Update(clean);
                Console.WriteLine("Assembly code updated");
            }
            #endregion

            #region find missing plugin types a registre these
            {
                var missings = (from s in steps where !pluginTypes.ContainsKey(s.Class.FullName) select s.Class.FullName).Distinct();
                foreach (var missing in missings)
                {
                    var type = new Entities.PluginType
                    {
                        PluginTypeId = Guid.NewGuid(),
                        TypeName = missing,
                        FriendlyName = Guid.NewGuid().ToString().ToLower(),
                        Name = missing,
                        PluginAssemblyId = plugin.ToEntityReference(),
                    };
                    uow.Create(type);
                    pluginTypes.Add(missing, type);
                    Console.WriteLine("Added plugin type " + type.TypeName);
                }
            }
            #endregion

            #region fetch sdk messages
            var sdkMessageIndex = (from s in uow.SdkMessages.GetQuery() select s).ToArray().ToDictionary(s => s.Name);
            #endregion

            #region find missing steps and registre these
            {
                var missings = (from s in steps where !stepindex.ContainsKey(s.UniqueName) select s).ToArray();
                foreach (var missing in missings)
                {
                    var SdkMessage = sdkMessageIndex[missing.EventType.ToString()];

                    OrganizationName.PluginRegistration.Entities.SdkMessageFilter filter = null;

                    if (string.IsNullOrEmpty(missing.SecondaryEntity))
                    {
                        filter = (from f in uow.SdkMessageFilters.GetQuery()
                                  where f.SdkMessageId.Id == SdkMessage.SdkMessageId
                                    && f.PrimaryObjectTypeCode == missing.PrimaryEntity
                                  select f).SingleOrDefault();
                    }
                    else
                    {
                        filter = (from f in uow.SdkMessageFilters.GetQuery()
                                  where f.SdkMessageId.Id == SdkMessage.SdkMessageId
                                    && f.PrimaryObjectTypeCode == missing.PrimaryEntity
                                    && f.SecondaryObjectTypeCode == missing.SecondaryEntity
                                  select f).SingleOrDefault();
                    }

                    var deployment = 0;
                    if (missing.Offline)
                    {
                        deployment = 2;
                    }

                    var step = new Entities.SdkMessageProcessingStep
                    {
                        SdkMessageProcessingStepId = Guid.NewGuid(),
                        Name = missing.Name,
                        Mode = new Microsoft.Xrm.Sdk.OptionSetValue(missing.Async),
                        Rank = missing.ExecutionOrder,
                        Stage = new Microsoft.Xrm.Sdk.OptionSetValue(missing.StageValue),
                        SupportedDeployment = new Microsoft.Xrm.Sdk.OptionSetValue(deployment),
                        EventHandler = pluginTypes[missing.Class.FullName].ToEntityReference(),
                        SdkMessageId = sdkMessageIndex[missing.EventType.ToString()].ToEntityReference(),
                        SdkMessageFilterId = filter != null ? filter.ToEntityReference() : null,
                    };

                    if (missing.Stage == Model.StageEnum.PostOperationAsyncWithDelete)
                    {
                        step.AsyncAutoDelete = true;
                    }


                    uow.Create(step);
                    stepindex.Add(step.UniqueName, step);
                    Console.WriteLine("Added step " + step.Name);
                }
                {
                    var edits = (from s in steps where stepindex.ContainsKey(s.UniqueName) select s).ToArray();
                    foreach (var edit in edits)
                    {
                        var step = stepindex[edit.UniqueName];
                        var deployment = 0;

                        if (edit.Offline)
                        {
                            deployment = 2;
                        }

                        if (step.SupportedDeployment.Value != deployment)
                        {
                            var clean = uow.SdkMessageProcessingSteps.Clean(step);
                            clean.SupportedDeployment = new Microsoft.Xrm.Sdk.OptionSetValue(deployment);
                            uow.Update(clean);
                            Console.WriteLine("Changed supported deployment for " + edit.Name + " > " + deployment);
                        }

                        if (edit.Stage == Model.StageEnum.PostOperationAsyncWithDelete && !(step.AsyncAutoDelete ?? false))
                        {
                            var clean = uow.SdkMessageProcessingSteps.Clean(step);
                            clean.AsyncAutoDelete = true;
                            uow.Update(clean);
                            Console.WriteLine("Changed async delete policy deployment for " + edit.Name + " > " + deployment);
                        }

                        if (edit.Stage == Model.StageEnum.PostOperationAsyncWithoutDelete && (step.AsyncAutoDelete ?? false))
                        {
                            var clean = uow.SdkMessageProcessingSteps.Clean(step);
                            clean.AsyncAutoDelete = false;
                            uow.Update(clean);
                            Console.WriteLine("Changed async deployment for " + edit.Name + " > " + deployment);
                        }


                    }
                }
            }
            #endregion

            #region fetch images
            var images = (from im in uow.SdkMessageProcessingStepImages.GetQuery()
                          join st in uow.SdkMessageProcessingSteps.GetQuery() on im.SdkMessageProcessingStepId.Id equals st.SdkMessageProcessingStepId
                          join pl in uow.PluginTypes.GetQuery() on st.EventHandler.Id equals pl.PluginTypeId
                          where pl.PluginAssemblyId.Id == plugin.PluginAssemblyId
                            && (im.Name == "preimage" || im.Name == "postimage") 
                          select im).ToArray();
            {
                var needImages = (from s in steps where (s.Preimage == true || s.Postimage == true) select s).ToArray();

                foreach (var needImage in needImages)
                {
                    SdkMessageProcessingStepImage image;
                    string imageAttributes;
                    var xrmStep = stepindex[needImage.UniqueName];

                    if (needImage.Preimage)
                    {
                        imageAttributes = needImage.PreimageAttributes != null && needImage.PreimageAttributes.Length > 0 ? string.Join(",", needImage.PreimageAttributes) : null;
                        image = (from im in images where im.SdkMessageProcessingStepId.Id == xrmStep.SdkMessageProcessingStepId.Value && im.Name == "preimage" select im).SingleOrDefault();

                        if(image == null)
                        {
                            image = CreteImageObject(xrmStep, needImage.MessagePropertyName, "preimage", 0, imageAttributes);
                            uow.Create(image);
                            Console.WriteLine("Pre image created " + needImage.Name);
                        }
                        else
                        {
                            UpdateImage(image, imageAttributes, needImage.Name, uow);
                        }
                    }
                    
                    if(needImage.Postimage)
                    {
                        imageAttributes = needImage.PostimageAttributes != null && needImage.PostimageAttributes.Length > 0 ? string.Join(",", needImage.PostimageAttributes) : null;
                        image = (from im in images where im.SdkMessageProcessingStepId.Id == xrmStep.SdkMessageProcessingStepId.Value && im.Name == "postimage" select im).SingleOrDefault();

                        if (image == null)
                        {
                            image = CreteImageObject(xrmStep, needImage.MessagePropertyName, "postimage", 1, imageAttributes);
                            uow.Create(image);
                            Console.WriteLine("post image created " + needImage.Name);
                        }
                        else
                        {
                            UpdateImage(image, imageAttributes, needImage.Name, uow);
                        }
                    }
                }
                var notNeededs = (from im in images where im.Relevant == false select im).ToArray();
                foreach (var notNeeded in notNeededs)
                {
                    uow.Delete(notNeeded);
                    Console.WriteLine("Preimage deleted for " + notNeeded.Name);
                }
            }
            #endregion          
        }

        /// <summary>
        /// Create image object of an entity bsed on the image type.
        /// </summary>
        /// <param name="xrmStep"></param>
        /// <param name="MessagePropertyName"></param>
        /// <param name="imageName"></param>
        /// <param name="imageType"></param>
        /// <param name="imageAttributes"></param>
        /// <returns> Image object of class SdkMessageProcessingStepImage </returns>
        private static SdkMessageProcessingStepImage CreteImageObject(SdkMessageProcessingStep xrmStep, string MessagePropertyName, string imageName, int imageType, string imageAttributes)
        {
            return new Entities.SdkMessageProcessingStepImage
            {
                SdkMessageProcessingStepImageId = Guid.NewGuid(),
                SdkMessageProcessingStepId = xrmStep.ToEntityReference(),
                Name = imageName,
                EntityAlias = imageName,
                MessagePropertyName = MessagePropertyName,
                ImageType = new Microsoft.Xrm.Sdk.OptionSetValue(imageType),
                Description = imageName,
                Relevant = true,
                Attributes1 = imageAttributes
            };
        }

        /// <summary>
        /// Update the existing image if the attributes are changed.
        /// </summary>
        /// <param name="image"> old image from dynamics </param>
        /// <param name="imageAttribute"> new image attributes </param>
        /// <param name="imageName">The name of the image </param>
        /// <param name="uow"> Iunitofwork object that is connected to dynamics </param>
        private static void UpdateImage(SdkMessageProcessingStepImage image, string imageAttribute, string imageName, IUnitOfWork uow)
        {
            var clean = uow.SdkMessageProcessingStepImages.Clean(image);          

            if (imageAttribute != image.Attributes1)
            {
                clean.Attributes1 = imageAttribute;
                uow.Update(clean);
                Console.WriteLine(imageName + " updated " + imageName + " :" + imageAttribute);
            }
            image.Relevant = true;
        }

        private static string GetPublicKeyTokenFromAssembly(Assembly assembly)
        {
            var bytes = assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0)
                return "None";

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++)
                publicKeyToken += string.Format("{0:x2}", bytes[i]);

            return publicKeyToken;
        }
    }
}
