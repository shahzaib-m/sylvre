using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class ComplexVariableReferenceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "create testvar = Test.var#",
           @"""use strict"";.*Test\.__var.*;")]
        [TestCase(
            "create testvar = Test.value.member#",
           @"""use strict"";.*Test\.value\.member.*;")]
        public void Should_Output_Valid_ComplexVarReference_MemberRef(string sylvreInput,
            string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }

        [TestCase(
            "create testvar = call Test.return()#",
           @"""use strict"";.*Test.*__return.*\(.*\).*;")]
        [TestCase(
            "create testvar = call Test.value.someMethod()#",
           @"""use strict"";.*Test\.value.*someMethod.*\(.*\).*;")]
        public void Should_Output_Valid_ComplexVarReference_FunctionRef(string sylvreInput,
            string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }

        [TestCase(
            "create testvar = testVar[20]#",
           @"""use strict"";.*testVar.*\[.*\].*;")]
        [TestCase(
            "create testvar = testVar[1][43]#",
           @"""use strict"";.*testVar.*\[.*\].*\[.*\].*;")]
        [TestCase(
            "create testvar = testVar[1][43][i+2]#",
           @"""use strict"";.*testVar.*\[.*\].*\[.*\].*\[.*\];")]
        public void Should_Output_Valid_ComplexVarReference_IndexRef(string sylvreInput,
            string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }
    }
}
