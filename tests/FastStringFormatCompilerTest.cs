using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using FastStringFormat;
using FastStringFormat.Parsing;
using System;
using System.Globalization;

namespace FastStringFormat.Test
{
    [TestClass]
    public class FastStringFormatCompilerTest
    {
        private readonly Mock<IFormatStringParser> mockFormatStringProvider = new Mock<IFormatStringParser>();

        [TestMethod]
        public void TestFormatSingleton()
        {
            // GIVEN a parser that returns a single segment
            mockFormatStringProvider.Setup(m => m.ParseFormatString("format-string", It.IsAny<IParsedStringBuilder>()))
                                    .Callback((string s, IParsedStringBuilder b) => b.AddTextSegment("result"));

            // WHEN the compiler is run
            FastStringFormatCompiler compiler = new FastStringFormatCompiler(mockFormatStringProvider.Object);
            Func<object, string> formatter = compiler.Compile<object>("format-string");

            // AND the generated formatter is run
            string result = formatter.Invoke(new object());

            // THEN the result is as expected
            Assert.AreEqual("result", result);
        }

        [TestMethod]
        public void TestFormatSmallSet()
        {
            // GIVEN a parser that returns a single segment
            mockFormatStringProvider.Setup(m => m.ParseFormatString("format-string", It.IsAny<IParsedStringBuilder>()))
                                    .Callback((string s, IParsedStringBuilder b) =>
                                    {
                                        b.AddTextSegment("result");
                                        b.AddTextSegment("2");
                                        b.AddTextSegment("3");
                                        b.AddTextSegment("4");
                                    });

            // WHEN the compiler is run
            FastStringFormatCompiler compiler = new FastStringFormatCompiler(mockFormatStringProvider.Object);
            Func<object, string> formatter = compiler.Compile<object>("format-string");

            // AND the generated formatter is run
            string result = formatter.Invoke(new object());

            // THEN the result is as expected
            Assert.AreEqual("result234", result);
        }

        [TestMethod]
        public void TestFormatLargeSet()
        {
            // GIVEN a parser that returns a single segment
            mockFormatStringProvider.Setup(m => m.ParseFormatString("format-string", It.IsAny<IParsedStringBuilder>()))
                                    .Callback((string s, IParsedStringBuilder b) =>
                                    {
                                        b.AddTextSegment("result");
                                        b.AddTextSegment("2");
                                        b.AddTextSegment("3");
                                        b.AddTextSegment("4");
                                        b.AddTextSegment("5");
                                        b.AddTextSegment("6");
                                        b.AddTextSegment("7");
                                    });

            // WHEN the compiler is run
            FastStringFormatCompiler compiler = new FastStringFormatCompiler(mockFormatStringProvider.Object);
            Func<object, string> formatter = compiler.Compile<object>("format-string");

            // AND the generated formatter is run
            string result = formatter.Invoke(new object());

            // THEN the result is as expected
            Assert.AreEqual("result234567", result);
        }

        // TODO test current culture is used
        // TODO exceptions thrown for null values
    }
}
