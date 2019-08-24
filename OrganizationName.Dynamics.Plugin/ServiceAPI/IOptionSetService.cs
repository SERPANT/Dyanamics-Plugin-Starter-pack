using Microsoft.Xrm.Sdk.Metadata;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrganizationName.Dynamics.Plugin.ServiceAPI
{
    interface IOptionSetService
    {
        OptionMetadata[] GetGlobalOptionSet(string optionSetName);
        int? GetOptionSetsValue(string optionName, string optionSetName);
        OptionSetMetadata GetLocalOptionSet(string entityLogicalName, string optionSetName);
        string GetLocalOptionSetLabel(string entityLogicalName, string optionSetName, int value);
        int GetLocalOptionSetsValue(string entityLogicalName, string optionSetName, string optionName);
    }
}
