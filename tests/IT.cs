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
    }
}
