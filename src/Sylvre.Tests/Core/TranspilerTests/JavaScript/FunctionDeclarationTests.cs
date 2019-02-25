using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class FunctionDeclarationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "function quickSort PARAMS num_array, low, high < >",
           @"""use strict"";[\n ]*function[\n ]+quickSort\(num_array,[\n ]*low,[\n ]*high\)[\n ]*{[\n ]*}")]
        [TestCase(
            "function quickSort < >",
           @"""use strict"";[\n ]*function[\n ]+quickSort\(\)[\n ]*{[\n ]*}")]
        [TestCase(
            "function quickSort PARAMS single < >",
           @"""use strict"";[\n ]*function[\n ]+quickSort\(single\)[\n ]*{[\n ]*}")]
        public void Should_Output_Valid_JavaScript_Function(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [TestCase("function quickSort PARAMS num_array, low, high < >")]
        [TestCase("function quickSort < >")]
        [TestCase("function quickSort PARAMS single < >")]
        public void Should_Not_Provide_Transpile_Errors(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            Assert.IsFalse(output.HasTranspileErrors);
        }

        [TestCase(
            "function var < >",
           @"""use strict"";function[\n ]+__var\(\)[\n ]*{[\n ]*}")]
        [TestCase(
            "function class < >",
           @"""use strict"";function[\n ]+__class\(\)[\n ]*{[\n ]*}")]
        [TestCase(
            "function return < >",
           @"""use strict"";function[\n ]+__return\(\)[\n ]*{[\n ]*}")]
        [TestCase(
            "function true < >",
           @"""use strict"";function[\n ]+__true\(\)[\n ]*{[\n ]*}")]
        public void Should_Append_Two_Underscores_If_FuncName_Is_Reserved_Keyword(string sylvreInput, string regexToMatch)
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
