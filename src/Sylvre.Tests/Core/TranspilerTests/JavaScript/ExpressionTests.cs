using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class ExpressionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "create var = 1+2#",
           @"""use strict"";[\n ]*.*1.*\+.*2.*[\n ]*;")]
        [TestCase(
            "create var = 1 + 2 - 1*3 / 3.2#",
           @"""use strict"";[\n ]*.*1.*\+.*2.*-.*1.*\*.*3.*/.*3.2.*[\n ]*;")]
        [TestCase(
            "create var = (1+2-9)#",
           @"""use strict"";[\n ]*.*\(.*1.*\+.*2.*-.*9.*\).*[\n ]*;")]
        [TestCase(
            "create var = 1+2 * (32-7)#",
           @"""use strict"";[\n ]*.*1.*\+.*2.*\(.*32.*-.*7.*\).*[\n ]*;")]
        public void Should_Output_Valid_JavaScript_Expression(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
    }
}
