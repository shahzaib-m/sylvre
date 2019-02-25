using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.ParserTests.JavaScript
{
    class VariableDeclarationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("create _testvar = 18#")]
        [TestCase("create __test_var = 18#")]
        [TestCase("create ___TestVar_ = 18#")]
        public void Should_Provide_Parse_Error_When_Variable_Name_Has_Underscore_Prefixes(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsTrue(program.HasParseErrors);
        }
    }
}
