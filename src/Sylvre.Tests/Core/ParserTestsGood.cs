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
        public void Should_Not_Throw_Parse_Exception_With_Good_Sylvre()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sylvre.Tests.Core.TestData.quickSort.syl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Assert.DoesNotThrow(() => Parser.ParseSylvreInput(reader.ReadToEnd()));
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
