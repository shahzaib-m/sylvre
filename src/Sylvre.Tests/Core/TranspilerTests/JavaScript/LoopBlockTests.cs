using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class LoopBlockTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Output_Valid_While_Loop_Block()
        {
            string sylvreInput = "loopwhile(FALSE) < >";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";while[\n ]*\(.*\)[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }

        [TestCase(
            "loopfor(create test=0# i LTHAN test2.length# i increment#) <>",
           @"""use strict"";[\n ]*for[\n ]*\(.*;.*;\)[\n ]*{[\n ]*}")]
        [TestCase(
            "loopfor(test=0# i LTHAN test2.length# increment i) <>",
           @"""use strict"";[\n ]*for[\n ]*\(.*;.*;\)[\n ]*{[\n ]*}")]
        public void Should_Output_Valid_For_Loop_Block(string sylvreInput, string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }
    }
}