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
    }
}