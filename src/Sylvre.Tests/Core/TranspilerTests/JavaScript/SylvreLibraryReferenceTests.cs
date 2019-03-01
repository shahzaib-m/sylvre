using System.Linq;

using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core.TranspilerTests.JavaScript
{
    class SylvreLibraryReferenceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(
            "create test = call Sylvre.Console.output(\"test string output123\")#",
           @"""use strict"";.*console.log\(.*\)[\n ]*;")]
        [TestCase(
            "call Sylvre.Console.refresh()#",
           @"""use strict"";[\n ]*console.clear\(\)[\n ]*;")]
        public void Should_Output_Valid_JavaScript_Console_Output(string sylvreInput, string jsRegex)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsFalse(output.HasTranspileErrors);

            StringAssert.IsMatch(jsRegex, output.TranspiledCode);
        }

        [TestCase("create var = Sylvre[213]#")]
        [TestCase("call Sylvre[213].something()#")]
        [TestCase("create var = Sylvre[213]#")]
        [TestCase("create var = Sylvre[213].prop#")]
        public void Should_Give_Error_When_Index_Reference_After_Sylvre_Library_Ref(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsTrue(output.HasTranspileErrors);

            Assert.AreEqual("An index reference is not allowed after the library reference.",
                output.TranspileErrors.First().Message);
        }

        [TestCase("create var = Sylvre.Console[213]#")]
        [TestCase("call Sylvre.Console[213].something()#")]
        [TestCase("create var = Sylvre.Console[213]#")]
        [TestCase("create var = Sylvre.Console[213].prop#")]
        public void Should_Give_Error_When_Index_Reference_After_Module_Ref(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsTrue(output.HasTranspileErrors);

            Assert.AreEqual("An index reference is not allowed after a module reference.",
                output.TranspileErrors.First().Message);
        }

        [TestCase("call Sylvre()#")]
        [TestCase("create var = Sylvre#")]
        [TestCase("var = Sylvre#")]
        public void Should_Give_Error_When_No_Module_Ref_After_Library_Ref(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsTrue(output.HasTranspileErrors);

            Assert.AreEqual("Missing a module name after the library reference.",
                output.TranspileErrors.First().Message);
        }

        [TestCase("call Sylvre.Console()#")]
        [TestCase("create var = Sylvre.Console#")]
        [TestCase("var = Sylvre.Console#")]
        public void Should_Give_Error_When_No_Member_Ref_After_Module_Ref(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsTrue(output.HasTranspileErrors);

            Assert.AreEqual("Missing a module member reference after module name.",
                output.TranspileErrors.First().Message);
        }

        [TestCase("call Sylvre.NonExist()#")]
        [TestCase("create var = Sylvre.Someother123#")]
        [TestCase("var = Sylvre.TestInvalidModule#")]
        public void Should_Give_Error_When_Invalid_Module_Ref_Is_Used(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsTrue(output.HasTranspileErrors);

            Assert.AreEqual("This Sylvre module does not exist.",
                output.TranspileErrors.First().Message);
        }

        [TestCase("call Sylvre.Console.nonexist()#")]
        [TestCase("create var = Sylvre.Console.unknown#")]
        [TestCase("var = Sylvre.Console.testnonexist1233#")]
        public void Should_Give_Error_When_Invalid_Module_Member_Ref_Is_Used(string sylvreInput)
        {
            SylvreProgram program = Parser.ParseSylvreInput(sylvreInput);
            Assert.IsFalse(program.HasParseErrors);

            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                program, TargetLanguage.Javascript);
            Assert.IsTrue(output.HasTranspileErrors);

            Assert.AreEqual("This Sylvre module member does not exist.",
                output.TranspileErrors.First().Message);
        }
    }
}
