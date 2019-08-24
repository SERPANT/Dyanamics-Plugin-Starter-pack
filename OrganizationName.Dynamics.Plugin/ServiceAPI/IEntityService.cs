using Microsoft.Xrm.Sdk;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrganizationName.Dynamics.Plugin.ServiceAPI
{
    interface IEntityService
    {
        void Create(Entity entityRecord);
        Entity CreateAndReturn(Entity entityRecord);

        void Update(Entity entityRecord);

        Entity GetById(string entityLogicalName, Guid id);
        Entity GetById(string entityLogicalName, Guid id, string[] columns);
    }
}
