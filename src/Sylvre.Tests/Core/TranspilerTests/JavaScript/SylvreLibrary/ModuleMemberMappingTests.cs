using Sylvre.Core.Transpilers.JavaScript.SylvreLibrary;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript.SylvreLibrary
{
    class ModuleMemberMappingTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("Console", "output")]
        [TestCase("Console", "refresh")]
        public void Should_Output_True_For_Valid_Sylvre_Module_Member(string module, string member)
        {
            bool exists = SylvreJavaScriptMappings.DoesSylvreMemberOfModuleExist(module, member);
            ClassicAssert.IsTrue(exists);
        }

        [TestCase("Console", "nonexist")]
        [TestCase("Console", "")]
        [TestCase("Console", "    ")]
        [TestCase("Console", "12345")]
        public void Should_Output_False_For_Invalid_Sylvre_Module_Member(string module, string member)
        {
            bool exists = SylvreJavaScriptMappings.DoesSylvreMemberOfModuleExist(module, member);
            ClassicAssert.IsFalse(exists);
        }

        [TestCase("Console", "output", "log")]
        [TestCase("Console", "refresh", "clear")]
        public void Should_Output_Equivalent_JavaScript_Module_Member_For_Sylvre_Module_Member(
            string module, string member, string expected)
        {
            string output = SylvreJavaScriptMappings.GetEquivalentJavaScriptMemberOfModule(
                module, member);
            ClassicAssert.AreEqual(expected, output);
        }
    }
}
