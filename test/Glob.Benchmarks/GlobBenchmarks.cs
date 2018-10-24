using System.IO;
using BenchmarkDotNet.Attributes;
using GlobExpressions.AST;

namespace GlobExpressions.Benchmarks
{
    public class GlobBenchmarks
    {
        private const string Pattern = "p?th/*a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].*";
        private Glob _compiled;
        private Glob _uncompiled;
        private Tree _segments;
        private string[] _pathSegments;

        public GlobBenchmarks()
        {
            this._compiled = new Glob(Pattern, GlobOptions.Compiled);
            this._uncompiled = new Glob(Pattern);

            this._segments = new Parser(Pattern).ParseTree();

            this._pathSegments = "pAth/fooooacbfa2vd4.txt".Split(Path.DirectorySeparatorChar);
        }

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
