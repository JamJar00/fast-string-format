using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastStringFormat.Test
{
    [TestClass]
    public class FastStringFormatIT
    {
        private class DataObject
        {
            public string Forename { get; set; }
            public string Surname { get; set; }
            public DateTime Dob { get; set; }

            public bool LikesCats { get; set; }

            public string? NullString { get; set; }
            public IFormattable? NullObject { get; set; }

            public DataObject(string forename, string surname, DateTime dob, bool likesCats)
            {
                Forename = forename;
                Surname = surname;
                Dob = dob;
                LikesCats = likesCats;
                NullString = null;
                NullObject = null;
            }
        }

        [TestMethod]
        [DataRow("Hi!", "Hi!")]
        [DataRow("{forename}", "Steve")]
        [DataRow("{forename}{surname}", "SteveIrwin")]
        [DataRow("No{surname}space!", "NoIrwinspace!")]
        [DataRow("{DOB}", "22/09/1962 00:00:00")]
        [DataRow("{DOB:yyyy-MM-dd}", "1962-09-22")]
        [DataRow("{forename} {surname} was born {DOB}. It is {likesCats} that he liked cats.", "Steve Irwin was born 22/09/1962 00:00:00. It is True that he liked cats.")]
        [DataRow("This is null: {nullString}", "This is null: ")]
        [DataRow("{nullString}", "")]
        [DataRow("{nullObject}", "")]
        [DataRow("{nullObject:yyyy-MM-dd}", "")]
        public void TestFormatString(string formatString, string expected)
        {
            // GIVEN a valid format string
            // WHEN compiled
            Func<DataObject, string> formatter = new FastStringFormatCompiler().Compile<DataObject>(formatString);

            // THEN the formatter is not null
            Assert.IsNotNull(formatter);

            // GIVEN an data object to format
            DataObject data = new DataObject("Steve", "Irwin", new DateTime(1962, 9, 22), true);

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
            Assert.ThrowsException<ArgumentNullException>(() => new FastStringFormatCompiler().Compile<DataObject>(null), "formatString");
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
            Assert.ThrowsException<FormatStringSyntaxException>(() => new FastStringFormatCompiler().Compile<DataObject>(formatString), message);
        }
    }
}
