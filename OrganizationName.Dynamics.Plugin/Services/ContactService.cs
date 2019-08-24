using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using OrganizationName.Dynamics.Plugin.DI;
using OrganizationName.Dynamics.Plugin.Entities;
using OrganizationName.Dynamics.Plugin.ServiceAPI;
using OrganizationName.Dynamics.Plugin.Constants.EntityFields;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace OrganizationName.Dynamics.Plugin.Services
{
    //Note: Required so that we can us the service in plugins.
    [Export(typeof(ServiceAPI.IContactService))]
    class ContactService: IContactService
    {
        [Import]
        public IOrganizationService service { get; set; }

        // Note: This is how we access other services that we have created.
        [Import]
        public IEntityService entityService { get; set; }

        public void Create(Contact contactRecord)
        {
            entityService.Create(contactRecord);
        }

        /// <summary>
        /// Create the record and return the newly created contact record.
        /// </summary>
        /// <param name="contactRecord"> Entities.Contact object that is to be created </param>
        /// <returns> Newly created contact record </returns>
        public Entities.Contact CreateAndRetrieve(Contact contactRecord)
        {
            return (entityService.CreateAndReturn(contactRecord)).ToEntity<Entities.Contact>();
        }

        /// <summary>
        /// Return contact record based on GUID.
        /// </summary>
        /// <param name="contactId"> Guid of contact record </param>
        /// <returns> Contact record </returns>
        public Entities.Contact GetById(Guid contactId)
        {
            return (entityService.GetById(Entities.Contact.EntityLogicalName, contactId)).ToEntity<Entities.Contact>();
        }

        /// <summary>
        /// Return contact record based on GUID with the specifed columns.
        /// </summary>
        /// <param name="contactId">  Guid of contact record </param>
        /// <param name="columns"> List of fields </param>
        /// <returns> Contact record </returns>
        public Entities.Contact GetById(Guid contactId, string[] columns)
        {
            return (entityService.GetById(Entities.Contact.EntityLogicalName, contactId, columns)).ToEntity<Entities.Contact>();
        }

        /// <summary>
        /// Update the contact record.
        /// </summary>
        /// <param name="contactRecord"> Entities.Contact object to be updated </param>
        public void Update(Contact contactRecord)
        {
            entityService.Update(contactRecord);
        }

        /// <summary>
        /// Note: (Just an example)
        /// Get all contact record related to specified contact id.
        /// </summary>
        /// <param name="accountId"> Account record Guid </param>
        /// <returns> Entity Collection </returns>
        public EntityCollection GetyByAccountId(Guid accountId)
        {
            QueryExpression query = new QueryExpression(Entities.Contact.EntityLogicalName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition(new ConditionExpression(ContactFields.Account, ConditionOperator.Equal, accountId));
            EntityCollection households = service.RetrieveMultiple(query);

            try
            {
                return service.RetrieveMultiple(query);
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve by account id:" + ex.Message);
            }
        }

        /// <summary>
        /// Note: (Just an example)
        /// Get all contact record related to specified contact id. But only retrieved the specifed fields.
        /// </summary>
        /// <param name="accountId"> Account Record Guid </param>
        /// <returns> Entity Collection </returns>
        public EntityCollection GetyByAccountId(Guid accountId, string[] columns)
        {
            QueryExpression query = new QueryExpression(Entities.Contact.EntityLogicalName);
            query.ColumnSet = new ColumnSet(columns);
            query.Criteria.AddCondition(new ConditionExpression(ContactFields.Account, ConditionOperator.Equal, accountId));
            EntityCollection households = service.RetrieveMultiple(query);

            try
            {
                return service.RetrieveMultiple(query);
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve by account id:" + ex.Message);
            }
        }
    }
}
