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
        [InlineData("*.txt", @"c:\windows\file.txt", "file.zip")]
        [InlineData("*.txt", "file.txt")]
        [InlineData("/some/dir/folder/foo.*", "/some/dir/folder/foo.txt")]
        [InlineData("/some/dir/folder/foo.*", "/some/dir/folder/foo.csv")]
        [InlineData("a_*file.txt", "a_bigfile.txt", "another_file.txt")]
        [InlineData("a_*file.txt", "a_file.txt")]
        [InlineData("*file.txt", "bigfile.txt")]
        [InlineData("*file.txt", "smallfile.txt")]
        public void TestWildcard(string pattern, string positiveMatch, string negativeMatch = null)
        {
            var glob = new Glob(pattern);

            if (positiveMatch != null)
                Assert.True(glob.IsMatch(positiveMatch));
            if (negativeMatch != null)
                Assert.False(glob.IsMatch(negativeMatch));
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

        [Theory]
        [InlineData("*fil[e-z].txt", "bigfile.txt", "smallfila.txt")]
        [InlineData("*fil[e-z].txt", "smallfilf.txt", "smallfilez.txt")]
        [InlineData("*file[1-9].txt", "bigfile1.txt", "smallfile0.txt")]
        [InlineData("*file[1-9].txt", "smallfile8.txt", "smallfilea.txt")]
        public void TestCharacterRange(string pattern, string positiveMatch, string negativeMatch = null)
        {
            var glob = new Glob(pattern);

            if (positiveMatch != null)
                Assert.True(glob.IsMatch(positiveMatch));
            if (negativeMatch != null)
                Assert.False(glob.IsMatch(negativeMatch));
        }

        [Theory]
        [InlineData("*file[abc].txt", "bigfilea.txt", "smallfiled.txt")]
        [InlineData("*file[abc].txt", "smallfileb.txt", "smallfileaa.txt")]
        [InlineData("*file[!abc].txt", "smallfiled.txt", "bigfilea.txt")]
        [InlineData("*file[!abc].txt", "smallfile-.txt", "smallfileaa.txt")]
        [InlineData("*file[!abc].txt", null, "smallfileb.txt")]
        public void TestCharacterList(string pattern, string positiveMatch, string negativeMatch = null)
        {
            var glob = new Glob(pattern);

            if (positiveMatch != null)
                Assert.True(glob.IsMatch(positiveMatch));
            if (negativeMatch != null)
                Assert.False(glob.IsMatch(negativeMatch));
        }

        [Fact]
        public void CanMatchDirectoryWildcardInTopLevelDirectory()
        {
            const string globPattern = @"/**/somefile";
            var glob = new Glob(globPattern);
            Assert.True(glob.IsMatch("/somefile"));
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

        [Fact]
        public void ShouldMatchCaseInsensitiveInCharacterSet()
        {
            const string globPattern = @"[a-z]";
            var glob = new Glob(globPattern, GlobOptions.CaseInsensitive);
            Assert.True(glob.IsMatch("A"));
            Assert.True(glob.IsMatch("B"));
            Assert.True(glob.IsMatch("Z"));
        }

        [Fact]
        public void ShouldMatchCaseInsensitiveInSegment()
        {
            const string globPattern = @"taco";
            var glob = new Glob(globPattern, GlobOptions.CaseInsensitive);
            Assert.True(glob.IsMatch("Taco"));
            Assert.True(glob.IsMatch("tAco"));
            Assert.True(glob.IsMatch("taCO"));
        }

        [Fact]
        public void ShouldMatchCaseInsensitiveInLiteralSet()
        {
            const string globPattern = @"{ab,cd}";
            var glob = new Glob(globPattern, GlobOptions.CaseInsensitive);
            Assert.True(glob.IsMatch("Ab"));
            Assert.True(glob.IsMatch("aB"));
            Assert.True(glob.IsMatch("AB"));
            Assert.True(glob.IsMatch("Cd"));
            Assert.True(glob.IsMatch("cD"));
            Assert.True(glob.IsMatch("CD"));
        }

        [Fact]
        public void ShouldNotMatchLiteralSet()
        {
            const string globPattern = @"{ab,cd}";
            var glob = new Glob(globPattern, GlobOptions.CaseInsensitive);
            Assert.False(glob.IsMatch("dc"));
        }

        [Theory]
        [InlineData("a**/*.cs", "ab/c.cs", "a/b/c.cs")]
        [InlineData("a**/*.cs", "a/c.cs")]
        [InlineData("**a/*.cs", "a/c.cs", "b/a/a.cs")]
        [InlineData("**a/*.cs", "ba/c.cs")]
        public void TestDoubleWildcard(string pattern, string positiveMatch, string negativeMatch = null)
        {
            var glob = new Glob(pattern);
            if (positiveMatch != null)
                Assert.True(glob.IsMatch(positiveMatch));

            if (negativeMatch != null)
                Assert.False(glob.IsMatch(negativeMatch));
        }
    }
}
