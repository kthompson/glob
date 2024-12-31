using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using GlobExpressions.AST;
using Xunit;
using Xunit.Abstractions;
using static GlobExpressions.Tests.TestHelpers;

namespace GlobExpressions.Tests;

public class PathTraverserTests
{
    private readonly ITestOutputHelper _printer;

    public PathTraverserTests(ITestOutputHelper printer)
    {
        _printer = printer;
    }

    [Fact]
    public void ShouldMatchStringWildcard()
    {
        // *
        var list = new DirectorySegment(new SubSegment[]
        {
                StringWildcard.Default,
        });

        Assert.True(list.MatchesSegment("", false));
        Assert.True(list.MatchesSegment("a", false));
        Assert.True(list.MatchesSegment("abc", false));
    }

    [Fact]
    public void ShouldMatchIdentWildcard()
    {
        // ab*cd
        var list = new DirectorySegment(new SubSegment[]
        {
                new Identifier("ab"),
                StringWildcard.Default,
                new Identifier("cd"),
        });

        Assert.True(list.MatchesSegment("abcd", false));
        Assert.True(list.MatchesSegment("abcdcd", false));
        Assert.True(list.MatchesSegment("ab123456cd", false));

        Assert.False(list.MatchesSegment("ab123456cd11", false));
        Assert.False(list.MatchesSegment("abcd1", false));
        Assert.False(list.MatchesSegment("abcdcd1", false));
        Assert.False(list.MatchesSegment("ab123456cd1", false));
    }

    [Fact]
    public void ShouldMatchLiteralSet()
    {
        // ab*{cd,ef}
        var list = new DirectorySegment(new SubSegment[]
        {
                new Identifier("ab"),
                StringWildcard.Default,
                new LiteralSet("cd", "ef"),
        });

        Assert.True(list.MatchesSegment("abcd", false));
        Assert.True(list.MatchesSegment("abcdcd", false));
        Assert.True(list.MatchesSegment("ab123456cd", false));

        Assert.True(list.MatchesSegment("abef", false));
        Assert.True(list.MatchesSegment("abcdef", false));
        Assert.True(list.MatchesSegment("ab123456ef", false));

        Assert.False(list.MatchesSegment("ab123456cd11", false));
        Assert.False(list.MatchesSegment("abcd1", false));
        Assert.False(list.MatchesSegment("abcdcd1", false));
        Assert.False(list.MatchesSegment("ab123456cd1", false));
    }

    [Fact]
    public void ShouldMatchCharacterWildcard()
    {
        // ab?
        var list = new DirectorySegment(new SubSegment[]
        {
                new Identifier("ab"),
                CharacterWildcard.Default
        });

        Assert.True(list.MatchesSegment("abc", false));
        Assert.True(list.MatchesSegment("abd", false));
        Assert.True(list.MatchesSegment("ab1", false));

        Assert.False(list.MatchesSegment("eab", false));
        Assert.False(list.MatchesSegment("abef", false));
        Assert.False(list.MatchesSegment("ab123456cd11", false));
        Assert.False(list.MatchesSegment("abcd1", false));
        Assert.False(list.MatchesSegment("abcdcd1", false));
        Assert.False(list.MatchesSegment("ab123456cd1", false));
    }

    [Fact]
    public void ShouldMatchCharacterSet()
    {
        // ab?[abc]
        var list = new DirectorySegment(new SubSegment[]
        {
                new Identifier("ab"),
                CharacterWildcard.Default,
                new CharacterSet("abc", false)
        });

        Assert.True(list.MatchesSegment("abca", false));
        Assert.True(list.MatchesSegment("abda", false));
        Assert.True(list.MatchesSegment("ab1a", false));

        Assert.True(list.MatchesSegment("abcb", false));
        Assert.True(list.MatchesSegment("abdb", false));
        Assert.True(list.MatchesSegment("ab1b", false));

        Assert.True(list.MatchesSegment("abcc", false));
        Assert.True(list.MatchesSegment("abdc", false));
        Assert.True(list.MatchesSegment("ab1c", false));
    }

    [Fact]
    public void ShouldMatchCharacterSetRange()
    {
        // ab?[a-c]
        var list = new DirectorySegment(new SubSegment[]
        {
                new Identifier("ab"),
                CharacterWildcard.Default,
                new CharacterSet("a-c", false)
        });

        Assert.True(list.MatchesSegment("abca", false));
        Assert.True(list.MatchesSegment("abda", false));
        Assert.True(list.MatchesSegment("ab1a", false));

        Assert.True(list.MatchesSegment("abcb", false));
        Assert.True(list.MatchesSegment("abdb", false));
        Assert.True(list.MatchesSegment("ab1b", false));

        Assert.True(list.MatchesSegment("abcc", false));
        Assert.True(list.MatchesSegment("abdc", false));
        Assert.True(list.MatchesSegment("ab1c", false));
    }

    [Fact]
    public void ShouldMatchCharacterSetInverted()
    {
        // ab?[!abc]
        var list = new DirectorySegment(new SubSegment[]
        {
                new Identifier("ab"),
                CharacterWildcard.Default,
                new CharacterSet("abc", true)
        });

        Assert.True(list.MatchesSegment("abcd", false));
        Assert.True(list.MatchesSegment("abdd", false));
        Assert.True(list.MatchesSegment("ab1d", false));
        Assert.True(list.MatchesSegment("abce", false));
        Assert.True(list.MatchesSegment("abde", false));
        Assert.True(list.MatchesSegment("ab1e", false));
        Assert.True(list.MatchesSegment("abcf", false));
        Assert.True(list.MatchesSegment("abdf", false));
        Assert.True(list.MatchesSegment("ab1f", false));

        Assert.False(list.MatchesSegment("abca", false));
        Assert.False(list.MatchesSegment("abda", false));
        Assert.False(list.MatchesSegment("ab1a", false));

        Assert.False(list.MatchesSegment("abcb", false));
        Assert.False(list.MatchesSegment("abdb", false));
        Assert.False(list.MatchesSegment("ab1b", false));

        Assert.False(list.MatchesSegment("abcc", false));
        Assert.False(list.MatchesSegment("abdc", false));
        Assert.False(list.MatchesSegment("ab1c", false));
    }

    [Fact]
    public void TraverseFiles()
    {
        var results = new DirectoryInfo(SourceRoot).Traverse("**/*Tests/**/P*", true, true, false).ToList();

        results.ForEach(file => _printer.WriteLine(file.FullName));

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void TraverseDirectories()
    {
        var results = new DirectoryInfo(SourceRoot).Traverse("**/*Tests/**/P*", true, false, true).ToList();

        results.ForEach(file => _printer.WriteLine(file.FullName));

        Assert.Single(results);
    }

    [Fact]
    public void TraverseFilesAndDirectories()
    {
        var results = new DirectoryInfo(Path.Combine(SourceRoot, "test")).Traverse("**/*Tests/**/P*", true, true, true).ToList();

        results.ForEach(file => _printer.WriteLine(file.FullName));

        Assert.Equal(4, results.Count);
    }

    [Theory]
    // Wildcard tests
    [InlineData("*.txt", "file.txt", "file.zip")]
    [InlineData("*.txt", "file.txt")]
    [InlineData("some/dir/folder/foo.*", "/some/dir/folder/foo.txt")]
    [InlineData("some/dir/folder/foo.*", "/some/dir/folder/foo.csv")]
    [InlineData("a_*file.txt", "a_bigfile.txt", "another_file.txt")]
    [InlineData("a_*file.txt", "a_file.txt")]
    [InlineData("*file.txt", "bigfile.txt")]
    [InlineData("*file.txt", "smallfile.txt")]

    // Character Range tests
    [InlineData("*fil[e-z].txt", "bigfile.txt", "smallfila.txt")]
    [InlineData("*fil[e-z].txt", "smallfilf.txt", "smallfilez.txt")]
    [InlineData("*file[1-9].txt", "bigfile1.txt", "smallfile0.txt")]
    [InlineData("*file[1-9].txt", "smallfile8.txt", "smallfilea.txt")]

    // CharacterList tests
    [InlineData("*file[abc].txt", "bigfilea.txt", "smallfiled.txt")]
    [InlineData("*file[abc].txt", "smallfileb.txt", "smallfileaa.txt")]
    [InlineData("*file[!abc].txt", "smallfiled.txt", "bigfilea.txt")]
    [InlineData("*file[!abc].txt", "smallfile-.txt", "smallfileaa.txt")]
    [InlineData("*file[!abc].txt", null, "smallfileb.txt")]

    // LiteralSet tests
    [InlineData("a{b,c}d", "abd", "a")]
    [InlineData("a{b,c}d", "acd")]

    // Root tests
    [InlineData("**/*.sln", "/mnt/e/code/csharp-glob/Glob.sln", "/mnt/e/code/csharp-glob/Glob.Tests/Glob.Tests.csproj")]
    [InlineData(@"**/*.txt", @"C:\Users\Kevin\Desktop\notes.txt", @"C:\Users\Kevin\Downloads\yarn-0.17.6.msi")]

    // Double wildcard tests
    [InlineData("a**/*.cs", "ab/c.cs", "a/b/c.cs")]
    [InlineData("a**/*.cs", "a/c.cs")]
    [InlineData("**a/*.cs", "a/c.cs", "b/a/a.cs")]
    [InlineData("**a/*.cs", "ba/c.cs")]
    [InlineData("**", "ba/c.cs")]
    [InlineData("**", "a")]
    [InlineData("**", "a/b")]
    [InlineData("a/**", "a/b/c")]
    [InlineData("**/somefile", "somefile")]
    public void TestGlobExpressions(string pattern, string? positiveMatch, string? negativeMatch = null)
    {
        var parser = new Parser(pattern);
        var segments = parser.ParseTree().Segments;

        var mockFileDatas = new Dictionary<string, MockFileData>();
        if (positiveMatch != null)
        {
            // Use an empty MockFileData to represent the file
            mockFileDatas[Path.Combine(FileSystemRoot, positiveMatch)] = new MockFileData(string.Empty);
        }

        if (negativeMatch != null)
        {
            // Use an empty MockFileData to represent the file
            mockFileDatas[Path.Combine(FileSystemRoot, negativeMatch)] = new MockFileData(string.Empty);
        }

        var cache = new MockTraverseOptions(true, true, false, new MockFileSystem(mockFileDatas));

        var root = new DirectoryInfo(FileSystemRoot);
        var results = PathTraverser.Traverse(root, segments, cache).ToArray();

        if (positiveMatch != null)
            Assert.Single(results);

        if (positiveMatch == null && negativeMatch != null)
            Assert.Empty(results);
    }


    [Theory]
    // Double wildcard tests
    [InlineData("**/a", @"ab/a/a.cs a/taco.cs b/taco.cs b/ab/a/hat.taco", @"ab\a a b\ab\a")]

    // Issue 52
    [InlineData("**/a/**/b", @"a/a/a/b", @"a\a\a\b")]
    [InlineData("**/a/**/b", @"a/a/a/a/b", @"a\a\a\a\b")]
    public void TestGlobExpressionsWithEmitDirectories(string pattern, string files, string matches)
    {
        var parser = new Parser(pattern);
        var segments = parser.ParseTree().Segments;

        var mockFileDatas = new Dictionary<string, MockFileData>();
        foreach (var file in files.Split(' '))
        {
            // Use an empty MockFileData to represent a file
            mockFileDatas[Path.Combine(FileSystemRoot, file)] = new MockFileData(string.Empty);
        }

        var cache = new MockTraverseOptions(false, true, true, new MockFileSystem(mockFileDatas));

        var root = new DirectoryInfo(FileSystemRoot);
        var results = PathTraverser.Traverse(root, segments, cache)
            .Select(file => file.FullName.Substring(FileSystemRoot.Length))
            .OrderBy(x => x)
            .ToArray();
        var fileMatches = matches
            .Split(' ')
            .Select(x => x.Replace('\\', Path.DirectorySeparatorChar))
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(fileMatches, results);
    }

}
