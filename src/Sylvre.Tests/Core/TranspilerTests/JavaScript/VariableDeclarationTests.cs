using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class VariableDeclarationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Provide_Transpile_Error_When_Declaring_Sylvre()
        {
            string sylvreInput = "\n\ncreate Sylvre = 25.21#";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            Assert.IsTrue(output.HasTranspileErrors);
        }

        [TestCase(
            "create testvar = 18#",
           @"""use strict"";var.+testvar.*=.*;")]
        [TestCase(
            "create test_var = 18#",
           @"""use strict"";var.+test_var.*=.*;")]
        [TestCase(
            "create TestVar_ = 18#",
           @"""use strict"";var.+TestVar_.*=.*;")]
        public void Should_Output_Valid_JavaScript_Variable_Declaration(string sylvreInput, string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }


        [TestCase(
            "create var = 18#",
           @"""use strict"";var.+__var.*=.*;")]
        [TestCase(
            "create class = 18#",
           @"""use strict"";var.+__class.*=.*;")]
        [TestCase(
            "create return = 18#",
           @"""use strict"";var.+__return.*=.*;")]
        [TestCase(
            "create true = 18#",
           @"""use strict"";var.+__true.*=.*;")]
        public void Should_Append_Two_Underscores_If_Is_Reserved_Keyword(string sylvreInput, string regexToMatch)
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
