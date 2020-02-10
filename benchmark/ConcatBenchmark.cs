using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace FastStringFormat.Benchmark
{
    public class ConcatBenchmark
    {
        private object o1;

        private string s1;
        
        [GlobalSetup]
        public void Setup()
        {
            o1 = new object();
            s1 = "Hello!";
        }

        [Benchmark]
        public string ConcatObject()
        {
            return string.Concat(o1, s1);
        }

        [Benchmark]
        public string ConcatObjectWithToString()
        {
            return string.Concat(o1.ToString(), s1);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Md5VsSha256>();
        }
    }
}
