using System;
using System.Linq;
using System.Collections.Generic;

using OrganizationName.Dynamics.Plugin.DI;
using OrganizationName.Dynamics.Plugin.ServiceAPI;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace OrganizationName.Dynamics.Plugin.Services
{
    [Export(typeof(ServiceAPI.IOptionSetService))]
    public class OptionSetService : IOptionSetService
    {
        [Import]
        public IOrganizationService service { get; set; }

        /// <summary>
        /// Returns global option set as array
        /// </summary>
        /// <param name="optionSetName"> name of option set </param>
        /// <returns> Array of OptionMetadata </returns>
        public OptionMetadata[] GetGlobalOptionSet(string optionSetName)
        {
            RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
            {
                Name = optionSetName
            };

            RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)service.Execute(retrieveOptionSetRequest);
            OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;
            OptionMetadata[] optionList = retrievedOptionSetMetadata.Options.ToArray();

            return optionList;
        }

        /// <summary>
        /// Returns an null or int value representing the option set value in the database.
        /// </summary>
        /// <param name="optionName"> text name of the option set</param>
        /// <param name="optionSetName"> the field name of optionset </param>
        /// <returns> null or int </returns>
        public int? GetOptionSetsValue(string optionName, string optionSetName)
        {
            OptionMetadata[] optionList = GetGlobalOptionSet(optionSetName);

            var option = Array.Find(optionList, item => item.Label.LocalizedLabels[0].Label == optionName);

            if (option != null)
            {
                return option.Value;
            }

            return null;
        }

        /// <summary>
        /// Get local option set of the provided entity.
        /// </summary>
        /// <param name="entityLogicalName"> Entity name </param>
        /// <returns> Optionset list </returns>
        public OptionSetMetadata GetLocalOptionSet(string entityLogicalName, string optionSetName)
        {
            RetrieveEntityRequest retrieveDetails = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entityLogicalName
            };
            RetrieveEntityResponse retrieveEntityResponseObj = (RetrieveEntityResponse)service.Execute(retrieveDetails);
            EntityMetadata metadata = retrieveEntityResponseObj.EntityMetadata;
            PicklistAttributeMetadata picklistMetadata = metadata.Attributes.FirstOrDefault(attribute => string.Equals(attribute.LogicalName, optionSetName, StringComparison.OrdinalIgnoreCase)) as PicklistAttributeMetadata;

            return picklistMetadata.OptionSet;
        }
        /// <summary>
        /// Get local option set text value
        /// </summary>
        /// <param name="entityLogicalName">Logical Name of the entity.</param>
        /// <param name="optionSetName">Name of the optionSet.</param>
        /// <param name="value">The value whose label is to be fetched.</param>
        /// <returns>string</returns>
        public string GetLocalOptionSetLabel(string entityLogicalName, string optionSetName, int value)
        {
            OptionSetMetadata options = GetLocalOptionSet(entityLogicalName, optionSetName);
            IList<OptionMetadata> OptionsList = (from o in options.Options
                                                 where o.Value.Value == value
                                                 select o).ToList();
            string optionsetLabel = (OptionsList.First()).Label.UserLocalizedLabel.Label;

            return optionsetLabel;
        }

        /// <summary>
        /// Returns the int value representing the option set value in the database.
        /// </summary>
        /// <param name="entityLogicalName">Logical Name of the entity</param>
        /// <param name="optionSetName">Name of the optionSet</param>
        /// <param name="optionName">Option name</param>
        /// <returns>int</returns>
        public int GetLocalOptionSetsValue(string entityLogicalName, string optionSetName, string optionName)
        {
            OptionSetMetadata options = GetLocalOptionSet(entityLogicalName, optionSetName);
            IList<OptionMetadata> OptionsList = (from o in options.Options
                                                 where o.Label.LocalizedLabels[0].Label == optionName
                                                 select o).ToList();
            int optionsetValue = (OptionsList.First()).Value.Value;

            return optionsetValue;
        }
    }
}
