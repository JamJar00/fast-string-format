using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastStringFormat.Test
{
    [TestClass]
    public class FastStringFormatIT
    {
        private class Coordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public Coordinates(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        private class DataObject
        {
            public string Forename { get; set; }
            public string Surname { get; set; }
            public DateTime Dob { get; set; }

            public bool LikesCats { get; set; }

            public string? NullString { get; set; }
            public Formattable? NullObject { get; set; }
            public Coordinates Coordinates { get; set;}

            public DataObject(string forename, string surname, DateTime dob, bool likesCats, Coordinates coordinates)
            {
                Forename = forename;
                Surname = surname;
                Dob = dob;
                LikesCats = likesCats;
                NullString = null;
                NullObject = null;
                Coordinates = coordinates;
            }
        }

        private class Formattable : IFormattable
        {
            public string ToString(string? format, IFormatProvider? formatProvider)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        [DataRow("Hi!", "Hi!")]
        [DataRow("{Forename}", "Steve")]
        [DataRow("{Forename}{Surname}", "SteveIrwin")]
        [DataRow("No{Surname}space!", "NoIrwinspace!")]
        [DataRow("{Dob}", "22/09/1962 00:00:00")]
        [DataRow("{Dob:yyyy-MM-dd}", "1962-09-22")]
        [DataRow("{Forename} {Surname} was born {Dob}. It is {LikesCats} that he liked cats.", "Steve Irwin was born 22/09/1962 00:00:00. It is True that he liked cats.")]
        [DataRow("This is null: {NullString}", "This is null: ")]
        [DataRow("{NullString}", "")]
        [DataRow("{NullObject}", "")]
        [DataRow("{NullObject:yyyy-MM-dd}", "")]
        [DataRow("Coordinates: {Coordinates.Latitude}, {Coordinates.Longitude}", "Coordinates: -26.835488321500016, 152.96309154093498")]
        [DataRow("{Dob.DayOfWeek}", "Saturday")]
        public void TestFormatString(string formatString, string expected)
        {
            // GIVEN a valid format string
            // WHEN compiled
            Func<DataObject, string> formatter = new FastStringFormatCompiler().Compile<DataObject>(formatString);

            // THEN the formatter is not null
            Assert.IsNotNull(formatter);

            // GIVEN an data object to format
            DataObject data = new DataObject(
                "Steve",
                "Irwin",
                new DateTime(1962, 9, 22),
                true,
                new Coordinates(-26.835488321500016, 152.96309154093498)
            );

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
#nullable disable
            Assert.ThrowsException<ArgumentNullException>(() => new FastStringFormatCompiler().Compile<DataObject>(null), "formatString");
#nullable enable
        }

        [TestMethod]
        public void TestArgumentExceptionIsThrownForNullFormatProviders()
        {
            // GIVEN a null format provider
            // WHEN compiled
            // THEN an exception is thrown
#nullable disable
            Assert.ThrowsException<ArgumentNullException>(() => new FastStringFormatCompiler().Compile<DataObject>("something", null), "formatProvider");
#nullable enable
        }

        [TestMethod]
        [DataRow("Hello, {Name", "Missing '}' to match '{' at position 7.")]
        [DataRow("Hello, {Name:something", "Missing '}' to match '{' at position 7.")]
        [DataRow("{}", "Empty parameter at position 0.")]
        [DataRow("{Name:}", "Empty format at position 0.")]
        [DataRow("{:something}", "Empty parameter at position 0.")]
        [DataRow("{NotFound}", "Property 'NotFound' not found on type 'FastStringFormat.Test.FastStringFormatIT+DataObject'. Does it have a public get accessor?")]
        [DataRow("{NotFound:something}", "Property 'NotFound' not found on type 'FastStringFormat.Test.FastStringFormatIT+DataObject'. Does it have a public get accessor?")]
        [DataRow("{LikesCats:something}", "Property 'LikesCats' does not return a type implementing IFormattable hence a format string cannot be applied to it.")]
        public void TestExceptionsAreThrownForInvalidFormatStrings(string formatString, string message)
        {
            // GIVEN an invalid format string
            // WHEN compiled
            // THEN an exception is thrown
            try {
                new FastStringFormatCompiler().Compile<DataObject>(formatString);
                Assert.Fail();
            } catch (FormatStringSyntaxException e) {
                Assert.AreEqual(message, e.Message);
            }
        }
    }
}
