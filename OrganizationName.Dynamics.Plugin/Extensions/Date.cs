using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationName.Dynamics.Plugin.Extensions.Date
{
   public static class Date
    {
        /// <summary>
        /// Returns difference between two date in years.
        /// </summary>
        public static double GetDiffInYear(DateTime startDate,DateTime endDate)
        {
            return ((endDate - startDate).TotalDays / 365);
        }
    }
}
