using System.Net;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastStringFormat.Test
{
    [TestClass]
    public class IT
    {
        class DataObject
        {
            public string Forename { get; set; }
            public string Surname { get; set; }
            public DateTime DOB { get; set; }
            
            public bool LikesCats { get; set; }
        }

        [TestMethod]
        [DataRow("Hi!", "Hi!")]
        [DataRow("{forename}", "Steve")]
        [DataRow("{forename}{surname}", "SteveIrwin")]
        [DataRow("No{surname}space!", "NoIrwinspace!")]
        [DataRow("{DOB}", "22/09/1962 00:00:00")]
        [DataRow("{DOB:yyyy-MM-dd}", "1962-09-22")]
        [DataRow("{forename} {surname} was born {DOB}. It is {likesCats} that he liked cats.", "Steve Irwin was born 22/09/1962 00:00:00. It is True that he liked cats.")]
        public void TestFormatString(string formatString, string expected)
        {
            // GIVEN a valid format string
            // WHEN compiled
            var formatter = new Compiler().Compile<DataObject>(formatString);

            // THEN the formatter is not null
            Assert.IsNotNull(formatter);

            // GIVEN an data object to format
            var data = new DataObject { Forename = "Steve", Surname = "Irwin", DOB = new DateTime(1962, 9, 22), LikesCats = true };

            // WHEN the formatter is invoked
            string result = formatter(data);

            // THEN the result is as expected
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestArgumentExceptionIsThrownForNullFormatStrings()
        {
            // GIVEN a null format string
            // WHEN compiled
            // THEN an exception is thrown
            Assert.ThrowsException<ArgumentNullException>(() => {
                new Compiler().Compile<DataObject>(null);
            }, "formatString");
        }

        [TestMethod]
        [DataRow("Hello, {Name", "Missing '}' to match '{' at position 8.")]
        [DataRow("Hello, {Name:something", "Missing '}' to match '{' at position 8.")]
        [DataRow("{}", "Empty parameter at position {openBraceAt}.")]
        [DataRow("{Name:}", "Empty format at position {openBraceAt}.")]
        [DataRow("{:something}", "Empty parameter at position {openBraceAt}.")]
        [DataRow("{NotFound}", "Property 'NotFound' not found on type 'DataObject'. Does it have a public get accessor?")]
        [DataRow("{NotFound:something}", "Property 'NotFound' not found on type 'DataObject'. Does it have a public get accessor?")]
        [DataRow("{LikesCats:something}", "Property 'LikesCats' does not return a type implementing IFormattable hence a format string cannot be applied to it.")]
        public void TestExceptionsAreThrownForInvalidFormatStrings(string formatString, string message)
        {
            // GIVEN an invalid format string
            // WHEN compiled
            // THEN an exception is thrown
            Assert.ThrowsException<FormatStringSyntaxException>(() => {
                new Compiler().Compile<DataObject>(formatString);
            }, message);
        }
    }
}
