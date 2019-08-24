using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationName.Dynamics.Plugin.DI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Import : Attribute
    {
    }
}
