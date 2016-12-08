using BenchmarkDotNet.Attributes;

namespace Glob.Benchmarks
{
    public class Benchmarks
    {
        private static readonly string Pattern = "p?th/*a[bcd]b[e-g]a[1-4][!wxyz][!a-c][!1-3].*";
        private Glob _compiled;
        private Glob _uncompiled;

        public Benchmarks()
        {
            this._compiled = new Glob(Pattern, GlobOptions.Compiled);
            this._uncompiled = new Glob(Pattern);
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

        [Benchmark]
        public void TestMatchForCompiledGlob()
        {
            var result = _compiled.IsMatch("pAth/fooooacbfa2vd4.txt");
        }

        [Benchmark]
        public void TestMatchForUncompiledGlob()
        {
            var result = _uncompiled.IsMatch("pAth/fooooacbfa2vd4.txt");
        }
    }
}
