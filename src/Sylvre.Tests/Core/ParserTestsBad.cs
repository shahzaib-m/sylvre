using System.IO;
using System.Linq;
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

        [TestCase("")]
        [TestCase("             ")]
        [TestCase("\t \t \n \n")]
        public void Should_Provide_Error_With_Empty_Input(string emptyInputVariations)
        {
            SylvreProgram result = Parser.ParseSylvreInput(emptyInputVariations);

            Assert.IsTrue(result.HasParseErrors);
            Assert.AreEqual(1, result.ParseErrors.Count);
        }

        [Test]
        public void Should_Have_Errors_Bad_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                Assert.IsTrue(result.HasParseErrors);
            }
        }

        [Test]
        public void Should_Provide_Errors_Within_ParseErrors_Property()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                Assert.IsNotNull(result.ParseErrors);
            }
        }

        [Test]
        public void Should_Provide_Specific_Amount_Of_Errors_Within_ParseErrors_Property()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                Assert.AreEqual(3, result.ParseErrors.Count);
            }
        }

        [Test]
        public void Should_Be_Specific_First_Error_Within_ParseErrors_Property()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort_bad_three_errors.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                SylvreParseError firstError = (SylvreParseError)result.ParseErrors.ElementAt(0);
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
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                SylvreParseError secondError = (SylvreParseError)result.ParseErrors.ElementAt(1);
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
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                SylvreParseError thirdError = (SylvreParseError)result.ParseErrors.ElementAt(2);
                Assert.IsFalse(thirdError.IsMismatchedInput);
                StringAssert.Contains("quickSort", thirdError.Message);
                Assert.AreEqual(53, thirdError.Line);
                Assert.AreEqual(9, thirdError.CharPositionInLine);
            }
        }
    }
}
