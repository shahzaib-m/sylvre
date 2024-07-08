using Sylvre.Core.Transpilers.JavaScript.SylvreLibrary;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript.SylvreLibrary
{
    class ModuleMappingTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("Console")]
        public void Should_Output_True_For_Valid_Sylvre_Module(string module)
        {
            bool exists = SylvreJavaScriptMappings.DoesSylvreModuleExist(module);
            ClassicAssert.IsTrue(exists);
        }

        [TestCase("NonExist")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("12345")]
        public void Should_Output_False_For_Invalid_Sylvre_Module(string module)
        {
            bool exists = SylvreJavaScriptMappings.DoesSylvreModuleExist(module);
            ClassicAssert.IsFalse(exists);
        }

        [TestCase("Console", "console")]
        public void Should_Output_Equivalent_JavaScript_Module_For_Sylvre_Module(string module, string expected)
        {
            string output = SylvreJavaScriptMappings.GetEquivalentJavaScriptModule(module);
            ClassicAssert.AreEqual(expected, output);
        }
    }
}
