using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace GlobExpressions.Benchmarks;

[MemoryDiagnoser]
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

    private static readonly string SourceRoot = Path.Combine("..", "..", "..", "..", "..");

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
    public Glob ParseAndCompileGlob() => new Glob(Pattern1, GlobOptions.Compiled);

    [Benchmark]
    public bool MatchForUncompiledGlob() => new Glob(Pattern1).IsMatch("pAth/fooooacbfa2vd4.txt");

    [Benchmark]
    public bool MatchForCompiledGlob() => compiled1.IsMatch("pAth/fooooacbfa2vd4.txt");

    [Benchmark]
    public bool MatchForUncompiledGlobDirectoryWildcard() => new Glob(Pattern2).IsMatch("pAth/fooooacbfa2vd4.txt");

    [Benchmark]
    public bool MatchForCompiledGlobDirectoryWildcard() => compiled1.IsMatch("pAth/fooooacbfa2vd4.txt");

    [Benchmark]
    public object BenchmarkParseToTree() => new Parser(Pattern1).ParseTree();

    [Benchmark]
    public object PathTraversal() => Glob.Files(SourceRoot, "test/*Tests/**/*.cs").ToList();

    // Directory wildcard that walks the whole tree but matches very few files.
    // Exercises ShouldIncludePredicate where most entries are filtered before any FileInfo is allocated.
    [Benchmark]
    public object PathTraversalSparseMatch() => Glob.Files(SourceRoot, "**/*.csproj").ToList();

    // Directory wildcard matching directories rather than files.
    [Benchmark]
    public object PathTraversalDirectories() => Glob.Directories(SourceRoot, "**/bin").ToList();
}
