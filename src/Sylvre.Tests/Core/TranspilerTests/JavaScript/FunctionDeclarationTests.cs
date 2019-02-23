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
            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                Parser.ParseSylvreInput(sylvreInput), TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [TestCase("function quickSort PARAMS num_array, low, high < >")]
        [TestCase("function quickSort < >")]
        [TestCase("function quickSort PARAMS single < >")]
        public void Should_Not_Provide_Transpile_Errors(string sylvreInput)
        {
            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
               Parser.ParseSylvreInput(sylvreInput), TargetLanguage.Javascript);

            Assert.IsFalse(output.HasTranspileErrors);
        }
    }
}
