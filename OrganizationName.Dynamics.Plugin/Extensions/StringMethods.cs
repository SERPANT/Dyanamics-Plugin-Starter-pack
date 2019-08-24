using System.Linq;

namespace OrganizationName.Dynamics.Plugin.Extensions.String
{
    /// <summary>
    /// Class that has method for string manupulation function.
    /// </summary>
    public static class StringMethods
    {
        /// <summary>
        /// Change the word to upper case
        /// </summary>
        /// <param name="value"> string word </param>
        /// <returns> string </returns>
        public static string UpperCaseWords(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var words = value.Split(new char[] { ' ', '\t' });
                words = (from w in words select w.UppercaseFirst()).ToArray();
                return string.Join(" ", words);
            }
            return null;
        }

        /// <summary>
        /// Capitalize the first letter of the word.
        /// </summary>
        /// <param name="value"> string word </param>
        /// <returns> string </returns>
        public static string UppercaseFirst(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 0)
            {
                if (value.Length == 1) return value.ToUpper();
                return value.Substring(0,1).ToUpper() + value.Substring(1).ToLower();
            }
            return value;
        }
    }
}
