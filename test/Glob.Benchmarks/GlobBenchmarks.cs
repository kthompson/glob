using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace GlobExpressions.Benchmarks
{
    [CsvExporter]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    [KeepBenchmarkFiles]
    [ArtifactsPath("../../../BenchmarkDotNet.Artifacts")]
    public class GlobBenchmarks
    {
        private Glob compiled1;
        private Glob compiled2;
        private const string Pattern1 = "p?th/*a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].*";
        private const string Pattern2 = "**/*a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].*";

        public GlobBenchmarks()
        {
            this.compiled1 = new Glob(Pattern1, GlobOptions.Compiled);
            this.compiled2 = new Glob(Pattern2, GlobOptions.Compiled);
        }

        [Benchmark]
        public void ParseGlob()
        {
            var parser = new Parser(Pattern1);
            parser.Parse();
        }

        [Benchmark]
        public Glob ParseAndCompileGlob()
        {
            return new Glob(Pattern1, GlobOptions.Compiled);
        }

        [Benchmark]
        public bool MatchForUncompiledGlob()
        {
            return new Glob(Pattern1).IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public bool MatchForCompiledGlob()
        {
            return compiled1.IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public bool MatchForUncompiledGlobDirectoryWildcard()
        {
            return new Glob(Pattern2).IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public bool MatchForCompiledGlobDirectoryWildcard()
        {
            return compiled1.IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public object BenchmarkParseToTree()
        {
            return new Parser(Pattern1).ParseTree();
        }

        [Benchmark]
        public object PathTraversal()
        {
            var SourceRoot = Path.Combine("..", "..", "..", "..", "..");
            return Glob.Files(SourceRoot, "test/*Tests/**/*.cs").ToList();
        }
    }
}
