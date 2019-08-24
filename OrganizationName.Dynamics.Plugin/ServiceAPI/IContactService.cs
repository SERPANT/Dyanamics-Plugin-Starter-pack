using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace OrganizationName.Dynamics.Plugin.ServiceAPI
{
    interface IContactService
    {
        void Update(Entities.Contact contactRecord);

        void Create(Entities.Contact contactRecord);
        Entities.Contact CreateAndRetrieve(Entities.Contact contactRecord);

        Entities.Contact GetById(Guid contactId);
        Entities.Contact GetById(Guid contactId, string[] columns);

        // Just for example.
        EntityCollection GetyByAccountId(Guid accountId);
        EntityCollection GetyByAccountId(Guid accountId, string[] columns);
    }
}
