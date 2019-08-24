using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using OrganizationName.Dynamics.Plugin.DI;
using OrganizationName.Dynamics.Plugin.Attributes;
using OrganizationName.Dynamics.Plugin.ServiceAPI;

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
    [Step( EventType = new CrmEventType[3] {
            CrmEventType.Create,
            CrmEventType.Update,
            CrmEventType.Delete },
        PrimaryEntity = Entities.Contact.EntityLogicalName, // Name of the entity.
        Stage = StageEnum.PostOperation)] // The stage at which the plugin executes.
    public class ContactPostO : AbstractBasePlugin
    {
        IContactService contactService;
        /// <summary>
        /// Starting point of plugin execution.
        /// </summary>
        /// <param name="pluginContext"> Used for accesssing service and the target object </param>
        protected override void Execute(IPluginContext pluginContext)
        {
            // Note: Retrieve a ContactSerivce object.
            contactService = pluginContext.GetService<IContactService>();

            Entities.Contact target;

            try
            {
                target = pluginContext.Target.ToEntity<Entities.Contact>();
            }
            catch
            {
                return;
            }

            if(pluginContext.EventType == CrmEventType.Create)
            {
                // Functions that perform perticular task.               
            }
            else if(pluginContext.EventType == CrmEventType.Delete)
            {
                // function that perform perticular tasks.
            }
            else if (pluginContext.EventType == CrmEventType.Update)
            {
                // function that perform perticular tasks.
            }
        }
    }
}
