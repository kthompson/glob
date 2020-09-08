using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace GlobExpressions.Tests
{
    public class GlobTests
    {
        [Fact]
        public void ShouldMatchFullGlob()
        {
            var glob = new Glob("Foo.txt");
            Assert.False(glob.IsMatch("SomeFoo.txt"));
        }

        [Theory]
        // Identifier tests
        [InlineData("$tf/", @"$tf/", "xtf")]

        // Wildcard tests
        [InlineData("*.txt", @"c:\windows\file.txt", "file.zip")]
        [InlineData("*.txt", "file.txt")]
        [InlineData("/some/dir/folder/foo.*", "/some/dir/folder/foo.txt")]
        [InlineData("/some/dir/folder/foo.*", "/some/dir/folder/foo.csv")]
        [InlineData("a_*file.txt", "a_bigfile.txt", "another_file.txt")]
        [InlineData("a_*file.txt", "a_file.txt")]
        [InlineData("*file.txt", "bigfile.txt")]
        [InlineData("*file.txt", "smallfile.txt")]
        [InlineData("a/*", "a/", "a")]
        [InlineData("*", "a")]
        [InlineData("*", "folder1/a")]
        [InlineData("~$*", "~$ ~$a ~$aa")]

        // Character Range tests
        [InlineData("[]-]", "] -")]
        [InlineData("[/]", null, "/ a b")]
        [InlineData("[!]-]", "a b c", "] -")]
        [InlineData("[a-]", "a -")]
        [InlineData("[!a-]", "b", "a -")]
        [InlineData("[-a]", "a -")]
        [InlineData("[!-a]", "b", "a -")]
        [InlineData("[--0]", "- . 0", "/")]
        [InlineData("[!--0]", "a b c", "- . 0 /")]
        [InlineData("[!]a-]", "b c d", "] a -")]
        [InlineData(@"[[?*]", @"[ ? *", "a b c")]
        [InlineData("*fil[e-z].txt", "bigfile.txt", "smallfila.txt")]
        [InlineData("*fil[e-z].txt", "smallfilf.txt", "smallfilez.txt")]
        [InlineData("*file[1-9].txt", "bigfile1.txt", "smallfile0.txt")]
        [InlineData("*file[1-9].txt", "smallfile8.txt", "smallfilea.txt")]

        // CharacterList tests
        [InlineData("*file[abc].txt", "bigfilea.txt", "smallfiled.txt")]
        [InlineData("file[]].txt", "file].txt")]
        [InlineData("*file[abc].txt", "smallfileb.txt", "smallfileaa.txt")]
        [InlineData("*file[!abc].txt", "smallfiled.txt", "bigfilea.txt")]
        [InlineData("*file[!abc].txt", "smallfile-.txt", "smallfileaa.txt")]
        [InlineData("*file[!abc].txt", null, "smallfileb.txt")]

        // LiteralSet tests
        [InlineData("a{b,c}d", "abd", "a")]
        [InlineData("a{b,c}d", "acd")]

        // Root tests
        [InlineData("/**/*.sln", "/mnt/e/code/csharp-glob/Glob.sln", "/mnt/e/code/csharp-glob/Glob.Tests/Glob.Tests.csproj")]
        [InlineData(@"C:/**/*.txt", @"C:\Users\Kevin\Desktop\notes.txt", @"C:\Users\Kevin\Downloads\yarn-0.17.6.msi")]

        // Double wildcard tests
        [InlineData("a**/*.cs", "ab/c.cs", "a/b/c.cs")]
        [InlineData("a**/*.cs", "a/c.cs")]
        [InlineData("**a/*.cs", "a/c.cs", "b/a/a.cs")]
        [InlineData("**a/*.cs", "ba/c.cs")]
        [InlineData("**", "ba/c.cs")]
        [InlineData("**", "a")]
        [InlineData("**", "a/b")]
        [InlineData("a/**", "a/b/c")]
        [InlineData("a/**", "a/", "a")]
        [InlineData("/**/somefile", "/somefile")]

        // Escape sequences
        [InlineData(@"\[a-d\]", "[a-d]", @"b c \ [ ]")]
        [InlineData(@"\{ab,bc\}", "{ab,bc}", @"ab bc")]
        [InlineData(@"hat\?", "hat?", "hata hatb")]
        [InlineData(@"hat\*", "hat*", "hata hatb hat hat/taco hata/taco")]
        public void TestGlobExpressions(string pattern, string? positiveMatch, string? negativeMatch = null)
        {
            var glob = new Glob(pattern, GlobOptions.MatchFilenameOnly);

            if (positiveMatch != null)
            {
                foreach (var match in positiveMatch.Split(' '))
                {
                    Assert.True(glob.IsMatch(match));
                }
            }

            if (negativeMatch != null)
            {
                foreach (var match in negativeMatch.Split(' '))
                {
                    Assert.False(glob.IsMatch(match));
                }
            }
        }

        [Fact]
        public void TestInputWithSpace()
        {
            var pattern = "Microsoft Visual Studio/2017";
            var expectedMatch = @"Microsoft Visual Studio\2017";

            Assert.True(Glob.IsMatch(expectedMatch, pattern));
        }

        [Fact]
        public void TestEscapeSequenceWithSpaces()
        {
            var pattern = @"Generated\ Files";
            var expectedMatch = "Generated Files";

            Assert.True(Glob.IsMatch(expectedMatch, pattern));
        }

        [Fact]
        public void CanMatchParensAndEqual()
        {
            // Issue https://github.com/kthompson/glob/issues/57
            var glob = new Glob(@"a/abc(v=ws.10).md", GlobOptions.CaseInsensitive);
            Assert.True(glob.IsMatch(@"a\abc(v=ws.10).md"));
        }

        [Fact]
        public void CanMatchSingleFileOnExtension()
        {
            var glob = new Glob("folder/*.txt");
            Assert.True(glob.IsMatch("folder/bigfile.txt"));
            Assert.True(glob.IsMatch("folder/smallfile.txt"));
            Assert.False(glob.IsMatch("folder/smallfile.txt.min"));
        }

        [Fact]
        public void CanMatchSingleFileWithAnyNameOrExtension()
        {
            var glob = new Glob("folder/*.*");
            Assert.True(glob.IsMatch("folder/bigfile.txt"));
            Assert.True(glob.IsMatch("folder/smallfile.txt"));
            Assert.True(glob.IsMatch("folder/smallfile.txt.min"));
        }

        [Fact]
        public void GlobShouldNotMatchAnythingForEmptyString()
        {
            const string globPattern = @"";
            var glob = new Glob(globPattern);
            Assert.False(glob.IsMatch("/somefile"));
            Assert.False(glob.IsMatch(""));
        }

        [Fact]
        public void ShouldNotMatchCaseInsensitiveInCharacterSet()
        {
            const string globPattern = @"[a-z]";
            var glob = new Glob(globPattern);
            Assert.False(glob.IsMatch("A"));
            Assert.False(glob.IsMatch("B"));
            Assert.False(glob.IsMatch("Z"));
        }

        [Fact]
        public void ShouldNotMatchCaseInsensitiveInSegment()
        {
            const string globPattern = @"taco";
            var glob = new Glob(globPattern);
            Assert.False(glob.IsMatch("Taco"));
            Assert.False(glob.IsMatch("tAco"));
            Assert.False(glob.IsMatch("taCO"));
        }

        [Fact]
        public void ShouldNotMatchCaseInsensitiveInLiteralSet()
        {
            const string globPattern = @"{ab,cd}";
            var glob = new Glob(globPattern);
            Assert.False(glob.IsMatch("Ab"));
            Assert.False(glob.IsMatch("aB"));
            Assert.False(glob.IsMatch("AB"));
            Assert.False(glob.IsMatch("Cd"));
            Assert.False(glob.IsMatch("cD"));
            Assert.False(glob.IsMatch("CD"));
        }

        [Theory]
        // Character sets
        [InlineData(@"[a-z]", "A B Z")]
        // Segments
        [InlineData(@"taco", "Taco tAco taCO")]
        // LiteralSets
        [InlineData(@"{ab,cd}", "Ab aB AB Cd cD CD")]
        // Roots
        [InlineData(@"C:/**/*.txt", @"c:\Users\Kevin\Desktop\notes.txt C:\Users\Kevin\Desktop\notes.txt")]
        public void CaseInsensitiveTests(string pattern, string matches)
        {
            var glob = new Glob(pattern, GlobOptions.CaseInsensitive);
            foreach (var expectedMatch in matches.Split(' '))
            {
                Assert.True(glob.IsMatch(expectedMatch));
            }
        }

        [Theory]
        [InlineData(@"?", "a b c", "folder1/d folder2/e")]
        [InlineData(@"*", "a b c", "folder1/d folder2/e")]
        public void FullPathOptionTests(string pattern, string matches, string? nonMatches = null)
        {
            var glob = new Glob(pattern);
            foreach (var expectedMatch in matches.Split(' '))
            {
                Assert.True(glob.IsMatch(expectedMatch));
            }

            if (nonMatches != null)
            {
                foreach (var expectedMatch in nonMatches.Split(' '))
                {
                    Assert.False(glob.IsMatch(expectedMatch));
                }
            }
        }

        [Fact]
        public void ShouldNotMatchLiteralSet()
        {
            const string globPattern = @"{ab,cd}";
            var glob = new Glob(globPattern, GlobOptions.CaseInsensitive);
            Assert.False(glob.IsMatch("dc"));
        }

        [Fact]
        public void DeleteDirectoriesUnderPath()
        {
            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "DeleteDirectoriesUnderPath");
            try
            {
                CreateFiles(testRoot, "ab/bin/a.cs ab/bin/sub/a.cs a/taco.cs b/taco.cs b/ab/a/hat.taco");

                // Verify files exist before
                Assert.True(File.Exists(Path.Combine(testRoot, "a/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/ab/a/hat.taco")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/sub/a.cs")));

                foreach (var dir in Glob.Directories(testRoot, "**/bin"))
                {
                    var path = Path.Combine(testRoot, dir);
                    Directory.Delete(path, true);
                }

                // Verify bin folder was deleted but nothing else
                Assert.True(File.Exists(Path.Combine(testRoot, "a/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/ab/a/hat.taco")));
                Assert.False(File.Exists(Path.Combine(testRoot, "ab/bin/a.cs")));
                Assert.False(File.Exists(Path.Combine(testRoot, "ab/bin/sub/a.cs")));
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        [Fact]
        public void StarStarDirectories()
        {
            Action<string> AssertEqual(string expected) => actual => Assert.Equal(expected, actual);

            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "StarStarDirectories");
            try
            {
                CreateFiles(testRoot, "ab/bin/a.cs ab/bin/sub/a.cs a/taco.cs b/taco.cs b/ab/a/hat.taco");

                // Verify files exist before
                Assert.True(File.Exists(Path.Combine(testRoot, "a/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/sub/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/ab/a/hat.taco")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/taco.cs")));

                Assert.Collection(Glob.Directories(testRoot, "**").OrderBy(x => x),
                    AssertEqual(""),
                    AssertEqual("a"),
                    AssertEqual("ab"),
                    AssertEqual(Path.Combine("ab", "bin")),
                    AssertEqual(Path.Combine("ab", "bin", "sub")),
                    AssertEqual("b"),
                    AssertEqual(Path.Combine("b", "ab")),
                    AssertEqual(Path.Combine("b", "ab", "a"))
                );
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        [Fact]
        public void StarStarFilesAndDirectories()
        {
            Action<string> AssertEqual(string expected) => actual => Assert.Equal(expected, actual);

            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "StarStarFilesAndDirectories");
            try
            {
                CreateFiles(testRoot, "ab/bin/a.cs ab/bin/sub/a.cs a/taco.cs b/taco.cs b/ab/a/hat.taco");

                // Verify files exist before
                Assert.True(File.Exists(Path.Combine(testRoot, "a/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/sub/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/ab/a/hat.taco")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/taco.cs")));

                Assert.Collection(Glob.FilesAndDirectories(testRoot, "**").OrderBy(x => x),
                    AssertEqual(""),
                    AssertEqual("a"),
                    AssertEqual(Path.Combine("a", "taco.cs")),
                    AssertEqual(Path.Combine("ab")),
                    AssertEqual(Path.Combine("ab", "bin")),
                    AssertEqual(Path.Combine("ab", "bin", "a.cs")),
                    AssertEqual(Path.Combine("ab", "bin", "sub")),
                    AssertEqual(Path.Combine("ab", "bin", "sub", "a.cs")),
                    AssertEqual("b"),
                    AssertEqual(Path.Combine("b", "ab")),
                    AssertEqual(Path.Combine("b", "ab", "a")),
                    AssertEqual(Path.Combine("b", "ab", "a", "hat.taco")),
                    AssertEqual(Path.Combine("b", "taco.cs"))
                );
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        [Fact]
        public void StarStarFiles()
        {
            Action<string> AssertEqual(string expected) => actual => Assert.Equal(expected, actual);

            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "StarStarFiles");
            try
            {
                CreateFiles(testRoot, "ab/bin/a.cs ab/bin/sub/a.cs a/taco.cs b/taco.cs b/ab/a/hat.taco");

                // Verify files exist before
                Assert.True(File.Exists(Path.Combine(testRoot, "a/taco.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "ab/bin/sub/a.cs")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/ab/a/hat.taco")));
                Assert.True(File.Exists(Path.Combine(testRoot, "b/taco.cs")));

                Assert.Collection(Glob.Files(testRoot, "**").OrderBy(x => x),
                    AssertEqual(Path.Combine("a", "taco.cs")),
                    AssertEqual(Path.Combine("ab", "bin", "a.cs")),
                    AssertEqual(Path.Combine("ab", "bin", "sub", "a.cs")),
                    AssertEqual(Path.Combine("b", "ab", "a", "hat.taco")),
                    AssertEqual(Path.Combine("b", "taco.cs"))
                );
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        [Fact]
        public void StarStarFilesIssue52()
        {
            Action<string> AssertEqual(string expected) => actual => Assert.Equal(expected, actual);

            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "StarStarFilesIssue52");
            try
            {
                CreateFiles(testRoot, "a/a/a/a/b.txt");

                // Verify files exist before
                Assert.True(File.Exists(Path.Combine(testRoot, "a/a/a/a/b.txt")));

                Assert.Collection(Glob.Files(testRoot, "**/a/**/b.txt").OrderBy(x => x),
                    AssertEqual(Path.Combine("a", "a", "a", "a", "b.txt"))
                );
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        [Fact]
        public void Issue59MultipleEnumerationsReturnSameResults()
        {
            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "Issue59MultipleEnumerationsReturnSameResults");
            try
            {
                CreateFiles(testRoot, "b/a taco.txt a/b");

                var allFiles = Glob.Files(testRoot, "**").OrderBy(x => x);
                var enumerator1 = allFiles.GetEnumerator();

                AssertEnumerator(enumerator1, Path.Combine("a", "b"));
                AssertEnumerator(enumerator1, Path.Combine("b", "a"));
                AssertEnumerator(enumerator1, "taco.txt");

                Assert.False(enumerator1.MoveNext());

                // second enumeration should emit same results
                var enumerator2 = allFiles.GetEnumerator();

                AssertEnumerator(enumerator2, Path.Combine("a", "b"));
                AssertEnumerator(enumerator2, Path.Combine("b", "a"));
                AssertEnumerator(enumerator2, "taco.txt");

                Assert.False(enumerator2.MoveNext());
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        [Fact]
        public void Issue59MissingFiles()
        {
            Action<string> AssertEqual(string expected) => actual => Assert.Equal(expected, actual);
            var testRoot = Path.Combine(Path.GetTempPath(), "Glob", "PathTraverserTests", "Issue59MissingFiles");
            try
            {
                CreateFiles(testRoot, "test1.txt deep/test2.txt");

                var allFiles = Glob.Files(testRoot, "**/*.txt").OrderBy(x => x).ToList();
                Assert.Collection(allFiles,
                    AssertEqual(Path.Combine("deep", "test2.txt")),
                    AssertEqual("test1.txt")
                );
            }
            finally
            {
                // Cleanup test
                Directory.Delete(testRoot, true);
            }
        }

        private static void AssertEnumerator(IEnumerator<string> enumerator, string expected)
        {
            Assert.True(enumerator.MoveNext());
            Assert.NotNull(enumerator.Current);
            Assert.Equal(expected, enumerator.Current);
        }

        private void CreateFiles(string testRoot, string files)
        {
            Directory.CreateDirectory(testRoot);

            foreach (var file in files.Split(' '))
            {
                var filePath = Path.Combine(testRoot, file);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.AppendAllText(filePath, "");
            }
        }
    }
}
