using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobooJson.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<KoobooJson>();
            BenchmarkRunner.Run<JILJson>();
            BenchmarkRunner.Run<JsonNet>();
            BenchmarkRunner.Run<UTF8Json>();
        }

    }
}
