using System.IO;
using System.Linq;
using System.Reflection;

using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;
using NUnit.Framework.Legacy;

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

            ClassicAssert.IsTrue(result.HasParseErrors);
            ClassicAssert.AreEqual(1, result.ParseErrors.Count);
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

                ClassicAssert.IsTrue(result.HasParseErrors);
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

                ClassicAssert.IsNotNull(result.ParseErrors);
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

                ClassicAssert.AreEqual(3, result.ParseErrors.Count);
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
                ClassicAssert.IsFalse(firstError.IsMismatchedInput);
                StringAssert.Contains("create temp num_array", firstError.Message);
                ClassicAssert.AreEqual(32, firstError.Line);
                ClassicAssert.AreEqual(13, firstError.CharPositionInLine);
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
                ClassicAssert.IsTrue(secondError.IsMismatchedInput);
                ClassicAssert.AreEqual(";", secondError.Symbol);
                ClassicAssert.AreEqual(46, secondError.Line);
                ClassicAssert.AreEqual(17, secondError.CharPositionInLine);
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
                ClassicAssert.IsFalse(thirdError.IsMismatchedInput);
                StringAssert.Contains("quickSort", thirdError.Message);
                ClassicAssert.AreEqual(53, thirdError.Line);
                ClassicAssert.AreEqual(9, thirdError.CharPositionInLine);
            }
        }
    }
}
