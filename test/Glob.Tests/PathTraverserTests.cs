using System;
using System.Collections.Generic;
using System.IO;
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
        var pathsToCreate = new[] { positiveMatch, negativeMatch }.Where(match => match != null).Select(match => match!);
        var testRoot = CreateTemporaryFileTree(pathsToCreate);
        try
        {
            var results = TraverseRelativeMatches(testRoot, pattern, caseSensitive: true, emitFiles: true, emitDirectories: false);

            if (positiveMatch != null)
                Assert.Contains(NormalizeRelativePath(positiveMatch), results);

            if (negativeMatch != null)
                Assert.DoesNotContain(NormalizeRelativePath(negativeMatch), results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Theory]
    // Double wildcard tests
    [InlineData("**/a", @"ab/a/a.cs a/taco.cs b/taco.cs b/ab/a/hat.taco", @"ab\a a b\ab\a")]

    // Issue 52
    [InlineData("**/a/**/b", @"a/a/a/b", @"a\a\a\b")]
    [InlineData("**/a/**/b", @"a/a/a/a/b", @"a\a\a\a\b")]
    public void TestGlobExpressionsWithEmitDirectories(string pattern, string files, string matches)
    {
        var pathsToCreate = files.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var testRoot = CreateTemporaryFileTree(pathsToCreate);
        try
        {
            var results = TraverseRelativeMatches(testRoot, pattern, caseSensitive: false, emitFiles: true, emitDirectories: true);
            var expectedMatches = matches
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizePath)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(expectedMatches, results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TestRootedPatternTraversal()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "rooted/path/target.sln", "rooted/path/target.csproj" });
        try
        {
            var fileSystemRoot = Path.GetPathRoot(testRoot)!;
            var positiveMatch = NormalizePath(Path.Combine(testRoot, "rooted/path/target.sln"));
            var negativeMatch = NormalizePath(Path.Combine(testRoot, "rooted/path/target.csproj"));
            var rootedPattern = NormalizeRelativePath(Path.GetRelativePath(fileSystemRoot, positiveMatch))
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');

            var results = TraverseFromRootMatches(fileSystemRoot, rootedPattern, caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Contains(positiveMatch, results);
            Assert.DoesNotContain(negativeMatch, results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalCanBeCaseInsensitive()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "Folder/File.TXT" });
        try
        {
            var caseSensitiveResults = TraverseRelativeMatches(testRoot, "folder/*.txt", caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Empty(caseSensitiveResults);

            var caseInsensitiveResults = TraverseRelativeMatches(testRoot, "folder/*.txt", caseSensitive: false, emitFiles: true, emitDirectories: false);
            Assert.Equal(new[] { NormalizePath("Folder/File.TXT") }, caseInsensitiveResults);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalWithNoEmitFlagsReturnsNoResults()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "a/b/c.txt" });
        try
        {
            var results = TraverseRelativeMatches(testRoot, "**", caseSensitive: true, emitFiles: false, emitDirectories: false);
            Assert.Empty(results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void MultiWildcardTraversalDoesNotEmitDuplicateFiles()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "a/b/target.txt" });
        try
        {
            var results = TraverseRelativeMatches(testRoot, "**/**/target.txt", caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Equal(new[] { NormalizePath("a/b/target.txt") }, results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void EmitDirectoriesOnlyReturnsMatchingDirectories()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "a/bin/file.txt", "a/bin/sub/deep.txt", "a/obj/file.txt" });
        try
        {
            var results = TraverseRelativeMatches(testRoot, "**/bin", caseSensitive: true, emitFiles: false, emitDirectories: true);
            Assert.Equal(new[] { NormalizePath("a/bin") }, results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalOnMissingRootReturnsEmpty()
    {
        var missingRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", Guid.NewGuid().ToString("N"), "missing");
        var results = TraverseRelativeMatches(missingRoot, "**/*.txt", caseSensitive: true, emitFiles: true, emitDirectories: true);
        Assert.Empty(results);
    }

    [Fact]
    public void TraversalSupportsEscapedAndSpecialCharacterNames()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "Generated Files/file[1].txt", ".config" });
        try
        {
            var bracketMatch = TraverseRelativeMatches(testRoot, @"Generated\ Files/file\[1\].txt", caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Equal(new[] { NormalizePath("Generated Files/file[1].txt") }, bracketMatch);

            var hiddenFileMatch = TraverseRelativeMatches(testRoot, ".config", caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Equal(new[] { ".config" }, hiddenFileMatch);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalSupportsLiteralSetWithEmptyOption()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "VbitResource_ById", "VbitResource_ByIds", "VbitResource_ByIda" });
        try
        {
            var results = TraverseRelativeMatches(testRoot, "VbitResource_ById{,s}", caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Equal(new[] { "VbitResource_ById", "VbitResource_ByIds" }, results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalCanReturnFileAndDirectoryWithSameName()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "a/x", "b/x/child.txt" });
        try
        {
            var results = TraverseRelativeMatches(testRoot, "**/x", caseSensitive: true, emitFiles: true, emitDirectories: true);
            Assert.Equal(new[] { NormalizePath("a/x"), NormalizePath("b/x") }, results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalCanBeReEnumeratedAfterPartialConsumption()
    {
        var testRoot = CreateTemporaryFileTree(new[] { "a/file.txt", "b/inner/file2.txt" });
        try
        {
            var segments = new Parser("**").ParseTree().Segments;
            var options = new TraverseOptions(caseSensitive: true, emitFiles: true, emitDirectories: true);
            var traversal = PathTraverser
                .Traverse(new DirectoryInfo(testRoot), segments, options)
                .Select(file => NormalizePath(Path.GetRelativePath(testRoot, file.FullName)));

            var firstEnumeration = new List<string>();
            using (var enumerator = traversal.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                firstEnumeration.Add(enumerator.Current);

                while (enumerator.MoveNext())
                    firstEnumeration.Add(enumerator.Current);
            }

            var secondEnumeration = traversal.ToList();
            Assert.Equal(firstEnumeration.OrderBy(x => x, StringComparer.Ordinal), secondEnumeration.OrderBy(x => x, StringComparer.Ordinal));
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void TraversalWithRecursiveSymlinkStillFindsRealFiles()
    {
        if (OperatingSystem.IsWindows())
            return;

        var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", Guid.NewGuid().ToString("N"));
        var realDirectory = Path.Combine(testRoot, "real");
        var loopLink = Path.Combine(realDirectory, "loop");
        Directory.CreateDirectory(realDirectory);
        File.AppendAllText(Path.Combine(realDirectory, "leaf.txt"), "");

        try
        {
            try
            {
                Directory.CreateSymbolicLink(loopLink, realDirectory);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or PlatformNotSupportedException or NotSupportedException)
            {
                return;
            }

            var segments = new Parser("**/*.txt").ParseTree().Segments;
            var options = new TraverseOptions(caseSensitive: true, emitFiles: true, emitDirectories: false);

            var results = PathTraverser
                .Traverse(new DirectoryInfo(testRoot), segments, options)
                .Select(file => NormalizePath(Path.GetRelativePath(testRoot, file.FullName)))
                .Take(20)
                .ToArray();

            Assert.Contains(NormalizePath("real/leaf.txt"), results);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [Fact]
    public void DeepWildcardTraversalOnLargerTreeReturnsEachFileOnce()
    {
        var paths = new List<string>();
        for (var i = 0; i < 20; i++)
        {
            paths.Add($"root{i}/a/b/file{i}.txt");
            paths.Add($"root{i}/a/c/other{i}.txt");
            paths.Add($"root{i}/x/y/ignore{i}.log");
        }

        var testRoot = CreateTemporaryFileTree(paths);
        try
        {
            var expected = paths
                .Where(path => path.EndsWith(".txt", StringComparison.Ordinal))
                .Select(NormalizePath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            var results = TraverseRelativeMatches(testRoot, "**/**/**/*.txt", caseSensitive: true, emitFiles: true, emitDirectories: false);
            Assert.Equal(expected, results);
            Assert.Equal(results.Length, results.Distinct(StringComparer.Ordinal).Count());

            var expectedDirectories = Enumerable.Range(0, 20)
                .Select(i => NormalizePath($"root{i}/a/b"))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            var directories = TraverseRelativeMatches(testRoot, "**/a/**/b", caseSensitive: true, emitFiles: true, emitDirectories: true);
            Assert.Equal(expectedDirectories, directories);
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    private static string[] TraverseRelativeMatches(string testRoot, string pattern, bool caseSensitive, bool emitFiles, bool emitDirectories)
    {
        var segments = new Parser(pattern).ParseTree().Segments;
        var options = new TraverseOptions(caseSensitive, emitFiles, emitDirectories);

        return PathTraverser
            .Traverse(new DirectoryInfo(testRoot), segments, options)
            .Select(file => NormalizePath(Path.GetRelativePath(testRoot, file.FullName)))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    private static string[] TraverseFromRootMatches(string rootPath, string pattern, bool caseSensitive, bool emitFiles, bool emitDirectories)
    {
        var segments = new Parser(pattern).ParseTree().Segments;
        var options = new TraverseOptions(caseSensitive, emitFiles, emitDirectories);

        return PathTraverser
            .Traverse(new DirectoryInfo(rootPath), segments, options)
            .Select(file => NormalizePath(file.FullName))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
    }

    private static string CreateTemporaryFileTree(IEnumerable<string> relativePaths)
    {
        var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testRoot);

        foreach (var path in relativePaths)
        {
            var normalizedPath = NormalizeRelativePath(path);
            var fullPath = Path.Combine(testRoot, normalizedPath);
            var directoryName = Path.GetDirectoryName(fullPath);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);
            File.AppendAllText(fullPath, "");
        }

        return testRoot;
    }

    private static string NormalizeRelativePath(string path) =>
        NormalizePath(path).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static string NormalizePath(string path) =>
        path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
}
