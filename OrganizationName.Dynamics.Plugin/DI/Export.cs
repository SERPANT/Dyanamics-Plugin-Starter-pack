using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OrganizationName.Dynamics.Plugin.DI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Export : Attribute
    {
        public Export(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; set; }
    }
}
