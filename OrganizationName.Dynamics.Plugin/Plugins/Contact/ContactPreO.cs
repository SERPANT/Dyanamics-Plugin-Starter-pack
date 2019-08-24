using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using OrganizationName.Dynamics.Plugin.DI;
using OrganizationName.Dynamics.Plugin.Attributes;

namespace OrganizationName.Dynamics.Plugin.Plugins.Contact
{
    /// <summary>
    /// 
    /// Plugin Tasks:
    ///     1. List of tasks that are included under this plugin  
    ///     
    /// </summary>
    /// 
    /// Detail about registering the plugin in dynamics
    [Step(EventType = new CrmEventType[3] {
            CrmEventType.Create,
            CrmEventType.Update,
            CrmEventType.Delete },
        PrimaryEntity = Entities.Contact.EntityLogicalName,
        Preimage = new bool [3]{false, true, false},
        Postimage = new bool[3] { false, false, true },
        Stage = StageEnum.PreOperation)] 
    public class ContactPreO: AbstractBasePlugin
    {
        /// <summary>
        /// Starting point of plugin execution.
        /// </summary>
        /// <param name="pluginContext"> Used for accesssing service and the target object </param>
        protected override void Execute(IPluginContext pluginContext)
        {
            Entities.Contact target;

            try
            {
                target = pluginContext.Target.ToEntity<Entities.Contact>();
            }
            catch
            {
                return;
            }

            if (pluginContext.EventType == CrmEventType.Create)
            {
                // Functions that perform perticular task.               
            }
            else if (pluginContext.EventType == CrmEventType.Delete)
            {
                Entities.Contact postImage = pluginContext.PostImage.ToEntity<Entities.Contact>();

                // function that perform perticular tasks.
            }
            else if (pluginContext.EventType == CrmEventType.Update)
            {
                Entities.Contact preImage = pluginContext.Preimage.ToEntity<Entities.Contact>();

                // function that perform perticular tasks.
            }
        }
    }
}
