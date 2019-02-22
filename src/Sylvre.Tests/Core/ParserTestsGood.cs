using System;
using System.IO;
using System.Reflection;

using Sylvre.Core;
using Sylvre.Core.Models;

using NUnit.Framework;

namespace Sylvre.Tests.Core
{
    class ParserTestsGood
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Not_Have_Errors_With_Good_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                Assert.IsFalse(result.HasParseErrors);
            }
        }

        [Test]
        public void Should_Have_Empty_ParseErrors_Property_With_Good_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                SylvreProgram result = Parser.ParseSylvreInput(reader.ReadToEnd());

                Assert.AreEqual(0, result.ParseErrors.Count);
            }
        }

        [Test]
        public void Should_Not_Return_Null_With_Good_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Assert.IsNotNull(Parser.ParseSylvreInput(reader.ReadToEnd()));
            }
        }

        [Test]
        public void Should_Return_SylvreProgram_With_Good_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Assert.IsInstanceOf(typeof(SylvreProgram), 
                    Parser.ParseSylvreInput(reader.ReadToEnd()));
            }
        }
    }
}
