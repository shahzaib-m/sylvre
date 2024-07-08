using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class VariableAssignmentTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Provide_Transpile_Error_When_Assigning_To_Sylvre()
        {
            string sylvreInput = "\nSylvre = 25.21#";
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            ClassicAssert.IsTrue(output.HasTranspileErrors);
        }

        [TestCase(
            "testvar = 18#",
           @"""use strict"";testvar.*=.*;")]
        [TestCase(
            "test_var = 18#",
           @"""use strict"";test_var.*=.*;")]
        [TestCase(
            "TestVar.someprop = 18#",
           @"""use strict"";TestVar\.someprop.*=.*;")]
        public void Should_Output_Valid_JavaScript_Variable_Assignment(string sylvreInput, string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            ClassicAssert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }

        [TestCase(
            "var = 18#",
           @"""use strict"";__var.*=.*;")]
        [TestCase(
            "class = 18#",
           @"""use strict"";__class.*=.*;")]
        [TestCase(
            "return = 18#",
           @"""use strict"";__return.*=.*;")]
        [TestCase(
            "true = 18#",
           @"""use strict"";__true.*=.*;")]
        public void Should_Append_Two_Underscores_If_Is_Reserved_Keyword(string sylvreInput, string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            ClassicAssert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }

        [TestCase(
            "var += 18#",
           @"""use strict"";.*\+=.*;")]
        [TestCase(
            "class -= somevar * 2#",
           @"""use strict"";.*-=.*;")]
        [TestCase(
            "return *= (var1.prop / 2)#",
           @"""use strict"";.*\*=.*;")]
        [TestCase(
            "true /= call someFunc()#",
           @"""use strict"";.*/=.*;")]
        public void Should_Output_Valid_JavaScript_Assignment_Operator(string sylvreInput, string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            ClassicAssert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }

        [TestCase(
            "var = []#",
           @"""use strict"";.*\=.*\[[\n ]*].*;")]
        [TestCase(
            "var = [ ]#",
           @"""use strict"";.*\=.*\[[\n ]*\].*;")]
        [TestCase(
            "var = [ 8, 17, 281, 1.2, 291.21, -21.2 ]#",
           @"""use strict"";.*\=.*\[.*,.*,.*,.*,.*,.*\].*;")]
        [TestCase(
            "var = [ \"8\", '17 test str', 281, 1.2, 291.21, -21.2 ]#",
           @"""use strict"";.*\=.*\[.*,.*,.*,.*,.*,.*\].*;")]
        public void Should_Output_Valid_JavaScript_Array_Assignment(string sylvreInput, string regexToMatch)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            ClassicAssert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);

            ClassicAssert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(regexToMatch, output.TranspiledCode);
        }
    }
}
