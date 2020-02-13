using System;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace FastStringFormat.Benchmark
{
    public class BenchmarkManySegmentFormatString
    {
        private static Random random = new Random();
        
        private Func<DataObject, string>? fsfFormatter;
        
        private StringBuilder? stringBuilder;

        private DataObject? dataObject;

        [GlobalSetup]
        public void GlobalSetup()
        {
            fsfFormatter = new FastStringFormatCompiler().Compile<DataObject>("Hello, {forename} {surname}.");
        }

        [IterationSetup]
        public void IterationSetup()
        {
            dataObject = new DataObject(RandomString(10), RandomString(10));
            
            stringBuilder = new StringBuilder();
        }

        [Benchmark]
        public string Concat()
        {
            return string.Concat("Hello, ", dataObject!.Forename, " ", dataObject!.Surname, ".");
        }

        [Benchmark]
        public string Addition()
        {
            // This compiles to the same as Concat, so should be identical in timings
            return "Hello, " + dataObject!.Forename + " " + dataObject!.Surname + ".";
        }

        [Benchmark]
        public string StringBuilder()
        {
            return stringBuilder!.Append("Hello, ").Append(dataObject!.Forename).Append(" ").Append(dataObject!.Surname).Append(".").ToString();
        }
        
        [Benchmark]
        public string StringFormat()
        {
            return string.Format("Hello, {0} {1}.", dataObject!.Forename, dataObject!.Surname);
        }
        
        [Benchmark]
        public string StringInterpolation()
        {
            // This compiles to the same as Concat, so should be identical in timings
            return $"Hello, {dataObject!.Forename} {dataObject!.Surname}.";
        }

        [Benchmark]
        public string FastStringFormat()
        {
            return fsfFormatter!(dataObject!);
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        class DataObject
        {
            public string Forename { get; set; }
            public string Surname { get; set; }

            public DataObject(string forename, string surname)
            {
                Forename = forename;
                Surname = surname;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkManySegmentFormatString>();
        }
    }
}
