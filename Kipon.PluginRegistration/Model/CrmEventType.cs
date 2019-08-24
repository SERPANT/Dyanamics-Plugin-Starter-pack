using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationName.PluginRegistration.Model
{
    public enum CrmEventType
    {
        Create,
        Update,
        Delete,
        Associate,
        Disassociate,
        SetState,
        SetStateDynamicEntity,
        RetrieveMultiple,
        Retrieve,
        Other,
        Close,
        AddMember,
        AddListMembers,
        RemoveMember,
        Merge
    }
}
