using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

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
            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                Parser.ParseSylvreInput("if () < >"), TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";if[\n ]*\(\)[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }

        [Test]
        public void Should_Output_Valid_ElseIf_Statement()
        {
            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                Parser.ParseSylvreInput("if () < > elseif () < >"), TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";.+else[\n ]+if[\n ]*\(\)[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }

        [Test]
        public void Should_Output_Valid_Else_Statement()
        {
            TranspileOutputBase output = Transpiler.TranspileSylvreToTarget(
                Parser.ParseSylvreInput("if () <> else < >"), TargetLanguage.Javascript);

            StringAssert.IsMatch(@"""use strict"";.+else[\n ]*{[\n ]*}",
                output.TranspiledCode);
        }
    }
}