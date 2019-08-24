using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationName.Dynamics.Plugin.Attributes
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
        AddMember,
        AddListMembers,
        RemoveMember,
        Merge
    }
}
