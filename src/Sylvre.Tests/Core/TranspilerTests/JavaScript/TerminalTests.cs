using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class TerminalTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "create var = 'test string single'#",
           @"""use strict"";.*'test string single';")]
        [TestCase(
            "create var = 'teststr'#",
           @"""use strict"";.*'teststr'.*;")]
        [TestCase(
            "create var = 'test string 12345 !#&|'#",
           @"""use strict"";.*'test string 12345 !#&|'.*;")]
        [TestCase(
            "create var = ' !#&| '#",
           @"""use strict"";.*' !#&| '.*;")]
        [TestCase(
            @"create var = 'escaped \'single\' quotes\'\''#",
            @"""use strict"";.*'escaped \\'single\\' quotes\\'\\''.*;")]
        public void Should_Output_Valid_JavaScript_Single_String(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [TestCase(
            "create var = \"test string single\"#",
           @"""use strict"";.*""test string single"";")]
        [TestCase(
            "create var = \"teststr\"#",
           @"""use strict"";.*""teststr"".*;")]
        [TestCase(
            "create var = \"test string 12345 !#&|\"#",
           @"""use strict"";.*""test string 12345 !#&|"".*;")]
        [TestCase(
            "create var = \" !#&| \"#",
           @"""use strict"";.*"" !#&| "".*;")]
        [TestCase(
            @"create var = ""escaped \""double\"" quotes\""\""""#",
            @"""use strict"";.*""escaped \\""double\\"" quotes\\""\\"""".*;")]
        public void Should_Output_Valid_JavaScript_Double_String(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [TestCase(
            "create var = TRUE#",
           @"""use strict"";.*true.*;")]
        [TestCase(
            "create var = FALSE#",
           @"""use strict"";.*false.*;")]
        [TestCase(
            "create var = TRUE AND var1#",
           @"""use strict"";.*true.*;")]
        [TestCase(
            "create var = var1 OR FALSE#",
           @"""use strict"";.*false.*;")]
        public void Should_Output_Valid_JavaScript_Boolean(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
    }
}
