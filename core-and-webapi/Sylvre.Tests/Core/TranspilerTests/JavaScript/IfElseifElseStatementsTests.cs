using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class IfElseifElseStatementsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Output_Valid_If_Statement()
        {
            string sylvreInput = "if (TRUE) < >";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";if[\n ]*\(.*\)[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }

        [Test]
        public void Should_Output_Valid_ElseIf_Statement()
        {
            string sylvreInput = "if (TRUE) < > elseif (TRUE) < >";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";.+else[\n ]+if[\n ]*\(.*\)[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }

        [Test]
        public void Should_Output_Valid_Else_Statement()
        {
            string sylvreInput = "if (TRUE) <> else < >";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";.+else[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }
    }
}