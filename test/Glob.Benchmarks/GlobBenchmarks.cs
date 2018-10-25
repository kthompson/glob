using System.IO;
using BenchmarkDotNet.Attributes;
using GlobExpressions.AST;

namespace GlobExpressions.Benchmarks
{
    [CsvExporter]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    [KeepBenchmarkFiles]
    [ArtifactsPath("../../../BenchmarkDotNet.Artifacts")]
    public class GlobBenchmarks
    {
        private const string Pattern = "p?th/*a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].*";

        [Benchmark]
        public void ParseGlob()
        {
            var parser = new Parser(Pattern);
            parser.Parse();
        }

        [Benchmark]
        public Glob ParseAndCompileGlob()
        {
            return new Glob(Pattern, GlobOptions.Compiled);
        }

        [Benchmark(Baseline = true)]
        public bool TestMatchForUncompiledGlob()
        {
            return new Glob(Pattern).IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public object BenchmarkParseToTree()
        {
            return new Parser(Pattern).ParseTree();
        }
    }
}
