using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class ConditionalExpressionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "create test = (-20 * -87) EQUALS 1234#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(.*\)==.*;")]
        [TestCase(
            "create test = (((20 + -87)) LTHAN (testvar2 + 38.02))#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(\(.*\(.*\)\).*<.*\(.*\)\)[\n ]*;")]
        [TestCase(
            "create test = ((20 + -87) LEQUAL (testvar2 + 38.02))#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(\(.*\).*<=.*\(.*\)\)[\n ]*;")]
        [TestCase(
            "create test = ((20 + -87)) GTHAN (testvar2 + 38.02)#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(.*\(.*\)\).*>.*\(.*\)[\n ]*;")]
        [TestCase(
            "create test = ((20 + -87)) GEQUAL (testvar2 + 38.02)#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(.*\(.*\)\).*>=.*\(.*\)[\n ]*;")]
        [TestCase(
            "create test = NOT var#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*!.*[\n ]*;")]
        [TestCase(
            "create test = NOT NOT var#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*!.*!.*[\n ]*;")]
        [TestCase(
            "create test = NOT NOT (var)#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*!.*!.*\(.*\)[\n ]*;")]
        [TestCase(
            "create test = (-20 * -87) AND (1234 / 23)#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(.*\)[\n ]*&&[\n ]*\(.*\)[\n ]*;")]
        [TestCase(
            "create test = (-20 + -87) OR (1234 - 23)#",
           @"""use strict"";[\n ]*var[\n ]+test[\n ]*=[\n ]*\(.*\)[\n ]*||[\n ]*\(.*\)[\n ]*;")]
        public void Should_Output_Valid_Conditional_Expression(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }
    }
}
