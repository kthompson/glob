using System;
using System.Collections.Generic;
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
        [InlineData(@"C:\**\*.txt", @"C:\Users\Kevin\Desktop\notes.txt", @"C:\Users\Kevin\Downloads\yarn-0.17.6.msi")]

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
        //[InlineData(@"\[a-d\]", "[a-d]", @"b c \ [ ]")]
        //[InlineData(@"\{ab,bc\}", "{ab,bc}", @"ab bc")]
        //[InlineData(@"hat\?", "hat?", "hata hatb")]
        //[InlineData(@"hat\*", "hat*", "hata hatb hat hat/taco hata/taco")]
        public void TestGlobExpressions(string pattern, string positiveMatch, string negativeMatch = null)
        {
            var glob = new Glob(pattern);

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
        [InlineData(@"C:\**\*.txt", @"c:\Users\Kevin\Desktop\notes.txt C:\Users\Kevin\Desktop\notes.txt")]
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
        public void FullPathOptionTests(string pattern, string matches, string nonMatches = null)
        {
            var glob = new Glob(pattern, GlobOptions.MatchFullPath);
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
    }
}
