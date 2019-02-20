using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core
{
    class ParserTestsBad
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Throw_Parse_Exception_With_Empty_Input()
        {
            Assert.Throws<SylvreParseException>(() =>
                Parser.ParseSylvreInput("             "));
        }

        [Test]
        public void Should_Throw_Parse_Exception_With_Bad_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Assert.Throws<SylvreParseException>(() =>
                    Parser.ParseSylvreInput(reader.ReadToEnd()));
            }
        }

        [Test]
        public void Should_Provide_Errors_Within_Parse_Exception()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var ex = Assert.Throws<SylvreParseException>(() =>
                            Parser.ParseSylvreInput(reader.ReadToEnd()));

                Assert.IsNotNull((List<SylvreParseError>)ex.Data["ParseErrors"]);
            }
        }

        [Test]
        public void Should_Provide_Specific_Amount_Of_Errors_Within_Parse_Exception()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var ex = Assert.Throws<SylvreParseException>(() => 
                            Parser.ParseSylvreInput(reader.ReadToEnd()));

                var expected = 3;
                var actual = ((List<SylvreParseError>)ex.Data["ParseErrors"]).Count;

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Should_Be_Specific_First_Error_Within_Parse_Exception()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var ex = Assert.Throws<SylvreParseException>(() =>
                            Parser.ParseSylvreInput(reader.ReadToEnd()));

                SylvreParseError firstError = ((List<SylvreParseError>)ex.Data["ParseErrors"])[0];
                Assert.IsFalse(firstError.IsMismatchedInput);
                StringAssert.Contains("create temp num_array", firstError.Message);
                Assert.AreEqual(32, firstError.Line);
                Assert.AreEqual(13, firstError.CharPositionInLine);
            }
        }

        [Test]
        public void Should_Be_Specific_Second_Error_Within_Parse_Exception()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var ex = Assert.Throws<SylvreParseException>(() =>
                            Parser.ParseSylvreInput(reader.ReadToEnd()));

                SylvreParseError secondError = ((List<SylvreParseError>)ex.Data["ParseErrors"])[1];
                Assert.IsTrue(secondError.IsMismatchedInput);
                Assert.AreEqual(";", secondError.Symbol);
                Assert.AreEqual(46, secondError.Line);
                Assert.AreEqual(17, secondError.CharPositionInLine);
            }
        }

        [Test]
        public void Should_Be_Specific_Third_Error_Within_Parse_Exception()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var ex = Assert.Throws<SylvreParseException>(() =>
                            Parser.ParseSylvreInput(reader.ReadToEnd()));

                SylvreParseError thirdError = ((List<SylvreParseError>)ex.Data["ParseErrors"])[2];
                Assert.IsFalse(thirdError.IsMismatchedInput);
                StringAssert.Contains("quickSort", thirdError.Message);
                Assert.AreEqual(53, thirdError.Line);
                Assert.AreEqual(9, thirdError.CharPositionInLine);
            }
        }
    }
}
