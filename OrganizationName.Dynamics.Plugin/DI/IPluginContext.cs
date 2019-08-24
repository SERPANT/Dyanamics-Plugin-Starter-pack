using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;

using OrganizationName.Dynamics.Plugin.Attributes;

namespace OrganizationName.Dynamics.Plugin.DI
{
    public interface IPluginContext
    {
        string UnsecureConfig { get; }
        string SecureConfig { get; }
        Guid UserId { get; }
        CrmEventType EventType { get; }
        Microsoft.Xrm.Sdk.Entity Target { get; }
        Microsoft.Xrm.Sdk.EntityReference TargetReference { get; }
        Microsoft.Xrm.Sdk.EntityReferenceCollection Associations { get; }
        Microsoft.Xrm.Sdk.Entity Preimage { get; }
        Microsoft.Xrm.Sdk.Entity PostImage { get; }
        Microsoft.Xrm.Sdk.Entity GetFullImage();
        bool AttributeChanged(params string[] names);
        T GetService<T>() where T: class;
        Guid PrimaryEntityId { get; }
    }
}
