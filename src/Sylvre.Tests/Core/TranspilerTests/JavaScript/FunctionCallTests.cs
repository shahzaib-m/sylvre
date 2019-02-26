using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class FunctionCallTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "call testFunction()#",
           @"""use strict"";[\n ]*.*\(\)[\n ]*;[\n ]*")]
        [TestCase(
            "call TestFunction()#",
           @"""use strict"";[\n ]*.*\(\)[\n ]*;[\n ]*")]
        [TestCase(
            "call test_func(arg)#",
           @"""use strict"";[\n ]*.*\(arg\)[\n ]*;[\n ]*")]
        [TestCase(
            "call testFunc123_45(arg1, arg2, arg3)#",
           @"""use strict"";[\n ]*.*\(arg1,arg2,arg3\)[\n ]*;[\n ]*")]
        public void Should_Output_Valid_JavaScript_Function_Call(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
    }
}
