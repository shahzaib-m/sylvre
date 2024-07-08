using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;
using NUnit.Framework.Legacy;

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
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [Test]
        public void Should_Output_Valid_JavaScript_Unary_Increment_Prefix()
        {
            string sylvreInput = "increment var#";
            string jsRegex = @"""use strict"";\+\+.*__var[n ]*;";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
        [Test]
        public void Should_Output_Valid_JavaScript_Unary_Increment_Suffix()
        {
            string sylvreInput = "var increment#";
            string jsRegex = @"""use strict"";__var.*\+\+[n ]*;";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [Test]
        public void Should_Output_Valid_JavaScript_Unary_Decrement_Prefix()
        {
            string sylvreInput = "decrement var#";
            string jsRegex = @"""use strict"";--.*__var[n ]*;";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
        [Test]
        public void Should_Output_Valid_JavaScript_Unary_Decrement_Suffix()
        {
            string sylvreInput = "var decrement#";
            string jsRegex = @"""use strict"";__var.*--[n ]*;";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
    }
}
