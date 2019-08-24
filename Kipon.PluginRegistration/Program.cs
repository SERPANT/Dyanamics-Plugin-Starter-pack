using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrganizationName.PluginRegistration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            string assemblyLocation = System.Configuration.ConfigurationManager.AppSettings["AssemblyLocation"];

            var uow = new OrganizationName.PluginRegistration.Entities.CrmUnitOfWork();
            var service = new OrganizationName.PluginRegistration.Services.PluginRegistrationService(uow);
            try
            {
                service.Registre(assemblyLocation);
            }
            catch(Exception ex)
            {
                Console.Write("Registering the plugins failed");
            }

        }
    }
}
