using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class FunctionReturnTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "exit#",
           @"""use strict"";[\n ]*return[\n ]*;")]
        [TestCase(
            "exit with i#",
           @"""use strict"";[\n ]*return.*[\n ]*;")]
        [TestCase(
            "exit with i + 329#",
           @"""use strict"";[\n ]*return.*[\n ]*;")]
        [TestCase(
            "exit with call somefunc()#",
           @"""use strict"";[\n ]*return.*[\n ]*;")]
        [TestCase(
            "exit with call testFunc123_45(arg1, arg2, arg3)#",
           @"""use strict"";[\n ]*return.*[\n ]*;")]
        public void Should_Output_Valid_JavaScript_Function_Return(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
    }
}
