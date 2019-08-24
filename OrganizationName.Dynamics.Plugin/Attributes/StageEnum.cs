using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationName.Dynamics.Plugin.Attributes
{
    public enum StageEnum
    {
        PreValidate,
        PreOperation,
        PostOperation,
        PostOperationAsyncWithDelete,
        PostOperationAsyncWithoutDelete,
    }
}
