using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Sylvre.Core.Transpilers.JavaScript
{
    /// <summary>
    /// A class for reserved keywords in JavaScript. Based on https://www.w3schools.com/js/js_reserved.asp.
    /// </summary>
    public static class JavaScriptReservedKeywords
    {
        /// <summary>
        /// The HashSet containing all the reserved keywords in JavaScript.
        /// </summary>
        private static readonly HashSet<string> _reservedKeywords = new HashSet<string>();

        /// <summary>
        /// Static constructor to initialise _reservedKeywords with the keywords in JavaScriptReservedKeywords.txt.
        /// </summary>
        static JavaScriptReservedKeywords()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Core.Transpilers.JavaScript.JavaScriptReservedKeywords.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string keyword;
                while ((keyword = reader.ReadLine()) != null)
                {
                    _reservedKeywords.Add(keyword);
                }
            }
        }

        /// <summary>
        /// Checks to see if the given input is a reserved JavaScript keyword.
        /// </summary>
        /// <param name="input">THe input string to check.</param>
        /// <returns>True if input is a reserved JavaScript keyword, false otherwise.</returns>
        public static bool IsReservedKeyword(string input)
        {
            return _reservedKeywords.Contains(input);
        }
    }
}
