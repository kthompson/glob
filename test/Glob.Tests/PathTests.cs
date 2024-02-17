using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GlobExpressions.Tests;

public class PathTests
{
    private static readonly string[] filenames = {
        @"C:\Users\Kevin\Desktop\notes.txt",
        @"C:\Users\Kevin\Downloads\yarn-0.17.6.msi",
        @"C:\Users\Kevin\Downloads\lotus_nightlight.stl",
        @"E:\code\csharp-glob\README.md",
        @"/mnt/e/code/csharp-glob/Glob/Glob.cs",
        @"/mnt/e/code/csharp-glob/Glob/Glob.csproj",
        @"/mnt/e/code/csharp-glob/Glob/GlobExtensions.cs",
        @"/mnt/e/code/csharp-glob/Glob/GlobNode.cs",
        @"/mnt/e/code/csharp-glob/Glob/GlobOptions.cs",
        @"/mnt/e/code/csharp-glob/Glob/GlobRunner.cs",
        @"/mnt/e/code/csharp-glob/Glob/GlobToRegexVisitor.cs",
        @"/mnt/e/code/csharp-glob/Glob/Parser.cs",
        @"/mnt/e/code/csharp-glob/Glob/Parser.cs.orig",
        @"/mnt/e/code/csharp-glob/Glob/Properties/AssemblyInfo.cs",
        @"/mnt/e/code/csharp-glob/Glob/Scanner.cs",
        @"/mnt/e/code/csharp-glob/Glob/Token.cs",
        @"/mnt/e/code/csharp-glob/Glob/TokenKind.cs",
        @"/mnt/e/code/csharp-glob/Glob.sln",
        @"/mnt/e/code/csharp-glob/Glob.Tests/Glob.Tests.csproj",
        @"/mnt/e/code/csharp-glob/Glob.Tests/Glob.Tests.csproj.user",
        @"/mnt/e/code/csharp-glob/Glob.Tests/GlobTests.cs",
        @"/mnt/e/code/csharp-glob/Glob.Tests/ParserTests.cs",
        @"/mnt/e/code/csharp-glob/Glob.Tests/Properties/AssemblyInfo.cs",
        @"/mnt/e/code/csharp-glob/Glob.Tests/RegexVisitorTests.cs",
        @"/mnt/e/code/csharp-glob/Glob.Tests/ScannerTests.cs",
        @"/mnt/e/code/csharp-glob/LICENSE",
        @"/mnt/e/code/csharp-glob/README.md",
    };

    [Fact]
    public void TestSimpleFilePattern()
    {
        Assert.Collection(GetGlobForPattern("*.txt"),
            s => Assert.Equal(@"C:\Users\Kevin\Desktop\notes.txt", s));
    }

    [Fact]
    public void RootPatternMatchWithoutDirectory()
    {
        Assert.Empty(GetGlobForPattern(@"C:/*.*"));
    }

    [Fact]
    public void LinuxRootWithDirectoryWildcard()
    {
        Assert.Collection(GetGlobForPattern(@"/**/*.sln"),
            s => Assert.Equal(@"/mnt/e/code/csharp-glob/Glob.sln", s));
    }

    [Fact]
    public void WindowsRootWithDirectoryWildcard()
    {
        Assert.Collection(GetGlobForPattern(@"C:/**/*.txt"),
            s => Assert.Equal(@"C:\Users\Kevin\Desktop\notes.txt", s));
    }

    private IEnumerable<string> GetGlobForPattern(string pattern)
    {
        var glob = new Glob(pattern, GlobOptions.Compiled | GlobOptions.MatchFilenameOnly);

        return filenames.Where(filename => glob.IsMatch(filename));
    }
}