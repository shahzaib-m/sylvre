using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Sylvre.Core.Transpilers.JavaScript.SylvreLibrary
{
    public static class SylvreJavaScriptMappings
    {
        /// <summary>
        /// The dictionary containing the Sylvre module as the key and the JavaScript equivalent as the value.
        /// E.g. "Console" = "console".
        /// </summary>
        private static readonly Dictionary<string, string> _moduleMappings =
            new Dictionary<string, string>();
        /// <summary>
        /// The dictionary containing the Sylvre module as the key and the dictionary containing the Sylvre module member and the JavaScript equivalent as the value.
        /// E.g. "Console" = [ "output" = "log", "refresh" = "clear" ].
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, string>> _moduleMemberMappings =
            new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Static constructor to initialise _reservedKeywords with the keywords in JavaScriptReservedKeywords.txt.
        /// </summary>
        static SylvreJavaScriptMappings()
        {
            // set up module mappings
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Core.Transpilers.JavaScript.SylvreLibrary.ModuleMappings.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string moduleMapping;
                while ((moduleMapping = reader.ReadLine()) != null)
                {
                    string[] splitted = moduleMapping.Split(':');
                    string sylvreModule = splitted[0];
                    string jsEquivalent = splitted[1];

                    _moduleMappings.Add(sylvreModule, jsEquivalent);
                    _moduleMemberMappings.Add(sylvreModule, new Dictionary<string, string>());
                }
            }

            // set up member mappings in modules
            resourceName = "Sylvre.Core.Transpilers.JavaScript.SylvreLibrary.ModuleMemberMappings.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string moduleMemberMapping;
                while ((moduleMemberMapping = reader.ReadLine()) != null)
                {
                    string[] splitted = moduleMemberMapping.Split(':');
                    string sylvreModule = splitted[0];
                    string sylvreModuleMember = splitted[1];
                    string jsEquivalentMember = splitted[2];

                    var module = _moduleMemberMappings[sylvreModule];
                    module.Add(sylvreModuleMember, jsEquivalentMember);
                }
            }
        }

        /// <summary>
        /// Checks to see if a given module is a valid Sylvre module. E.g. Console = true, Nonexist = false.
        /// </summary>
        /// <param name="sylvreModule">The Sylvre module to check for validity.</param>
        /// <returns>Whether the Sylvre module is valid or not.</returns>
        public static bool DoesSylvreModuleExist(string sylvreModule)
        {
            return _moduleMappings.ContainsKey(sylvreModule);
        }
        /// <summary>
        /// Returns the string of the equivalent JavaScript module by the given Sylvre module (e.g. "Console" will return "console").
        /// Precondition: DoesSylvreModuleExist().
        /// </summary>
        /// <param name="sylvreModule">The Sylvre module to get the equivalent JS module of.</param>
        /// <returns>The equivalent JS module name.</returns>
        public static string GetEquivalentJavaScriptModule(string sylvreModule)
        {
            return _moduleMappings[sylvreModule];
        }

        /// <summary>
        /// Checks to see if a given module member is a valid Sylvre module member within a Sylvre module. 
        /// E.g. module Console member "output" = true, module Console member "nonexist" = false.
        /// Precondition: DoesSylvreModuleExist().
        /// </summary>
        /// <param name="sylvreModule">The Sylvre module to look for the member within.</param>
        /// <param name="sylvreModuleMember">The Sylvre module member to check for validity.</param>
        /// <returns>Whether the Sylvre module member is valid or not.</returns>
        public static bool DoesSylvreMemberOfModuleExist(string sylvreModule, string sylvreModuleMember)
        {
            return _moduleMemberMappings[sylvreModule].ContainsKey(sylvreModuleMember);
        }
        /// <summary>
        /// Returns the string of the equivalent JavaScript module by the given Sylvre module (e.g. if module is "Console", "output" will return "log").
        /// Precondition: DoesSylvreMemberOfModuleExist().
        /// </summary>
        /// <param name="sylvreModule">The Sylvre module to look for the member within.</param>
        /// <param name="sylvreModuleMember">The Sylvre module member to get the equivalent JS module member of.</param>
        /// <returns>The equivalent JS module member name.</returns>
        public static string GetEquivalentJavaScriptMemberOfModule(string sylvreModule, string sylvreModuleMember)
        {
            return _moduleMemberMappings[sylvreModule][sylvreModuleMember];
        }
    }
}
