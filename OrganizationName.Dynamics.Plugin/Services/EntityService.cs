using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using OrganizationName.Dynamics.Plugin.ServiceAPI;

using OrganizationName.Dynamics.Plugin.DI;
using OrganizationName.Dynamics.Plugin.Constants.ErrorMessages;

namespace OrganizationName.Dynamics.Plugin.Services
{
    public class EntityService : IEntityService
    {
        [Import]
        public IOrganizationService service { get; set; }

        /// <summary>
        /// Create the entity passed to it.
        /// </summary>
        /// <param name="entityRecord"> Entity Object </param>
        public void Create(Entity entityRecord)
        {
            try
            {
                service.Create(entityRecord);
            }
            catch
            {
                throw new InvalidPluginExecutionException(OperationErrorMessages.CreateFailed + entityRecord.LogicalName);
            }
        }

        /// <summary>
        /// Create the entity passed and also return the newly created entity.
        /// </summary>
        /// <param name="entityRecord"> Entity Object </param>
        public Entity CreateAndReturn(Entity entityRecord)
        {
            Guid recordGuid = service.Create(entityRecord);
            return GetById(entityRecord.LogicalName, recordGuid);
        }

        /// <summary>
        /// Get the entity record based on id.
        /// </summary>
        /// <param name="entityLogicalName"> The entity name whose record is to be retrieved </param>
        /// <param name="id"> Guid of the record </param>
        /// <param name="columns"> the columns that are to be retrieved </param>
        /// <returns> Entity record </returns>
        public Entity GetById(string entityLogicalName, Guid id, string[] columns)
        {
            return service.Retrieve(entityLogicalName, id, new ColumnSet(columns));
        }

        /// <summary>
        /// Get the entity record based on id with all the fields.
        /// </summary>
        /// <param name="entityLogicalName"> The entity name whose record is to be retrieved </param>
        /// <param name="id"> Guid of the record </param>
        /// <returns> Entity record </returns>
        public Entity GetById(string entityLogicalName, Guid id)
        {
            try
            {
                return service.Retrieve(entityLogicalName, id, new ColumnSet(true));
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationErrorMessages.RetrieveFailed + ex.Message);
            }
        }

        /// <summary>
        /// Update the entity.
        /// </summary>
        /// <param name="entityRecord"> Entity Record </param>
        public void Update(Entity entityRecord)
        {
            try
            {
                service.Update(entityRecord);
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationErrorMessages.UpdateFailed + ex.Message);
            }
        }
    }
}
