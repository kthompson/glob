using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Running;

namespace Glob.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ManualConfig();
            config.Add(DefaultConfig.Instance);
            config.Set(config.GetSummaryStyle().WithTimeUnit(TimeUnit.Nanosecond));

            BenchmarkRunner.Run<Benchmarks>(config);
        }
    }
}
