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
            Assert.False(glob.IsMatch("SomeFoo.txt")); // this fails because IsMatch is true
        }

        [Fact]
        public void CanParseSimpleFilename()
        {
            var glob = new Glob("*.txt");
            Assert.True(glob.IsMatch("file.txt"));
            Assert.False(glob.IsMatch("file.zip"));
            Assert.True(glob.IsMatch(@"c:\windows\file.txt"));
        }

        [Fact]
        public void CanParseDots()
        {
            var glob = new Glob("/some/dir/folder/foo.*");
            Assert.True(glob.IsMatch("/some/dir/folder/foo.txt"));
            Assert.True(glob.IsMatch("/some/dir/folder/foo.csv"));
        }

        [Fact]
        public void CanMatchUnderscore()
        {
            var glob = new Glob("a_*file.txt");
            Assert.True(glob.IsMatch("a_bigfile.txt"));
            Assert.True(glob.IsMatch("a_file.txt"));
            Assert.False(glob.IsMatch("another_file.txt"));
        }

        [Fact]
        public void CanMatchSingleFile()
        {
            var glob = new Glob("*file.txt");
            Assert.True(glob.IsMatch("bigfile.txt"));
            Assert.True(glob.IsMatch("smallfile.txt"));
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
        public void CanMatchSingleFileUsingCharRange()
        {
            var glob = new Glob("*fil[e-z].txt");
            Assert.True(glob.IsMatch("bigfile.txt"));
            Assert.True(glob.IsMatch("smallfilf.txt"));
            Assert.False(glob.IsMatch("smallfila.txt"));
            Assert.False(glob.IsMatch("smallfilez.txt"));
        }

        [Fact]
        public void CanMatchSingleFileUsingNumberRange()
        {
            var glob = new Glob("*file[1-9].txt");
            Assert.True(glob.IsMatch("bigfile1.txt"));
            Assert.True(glob.IsMatch("smallfile8.txt"));
            Assert.False(glob.IsMatch("smallfile0.txt"));
            Assert.False(glob.IsMatch("smallfilea.txt"));
        }

        [Fact]
        public void CanMatchSingleFileUsingCharList()
        {
            var glob = new Glob("*file[abc].txt");
            Assert.True(glob.IsMatch("bigfilea.txt"));
            Assert.True(glob.IsMatch("smallfileb.txt"));
            Assert.False(glob.IsMatch("smallfiled.txt"));
            Assert.False(glob.IsMatch("smallfileaa.txt"));
        }

        [Fact]
        public void CanMatchSingleFileUsingInvertedCharList()
        {
            var glob = new Glob("*file[!abc].txt");
            Assert.False(glob.IsMatch("bigfilea.txt"));
            Assert.False(glob.IsMatch("smallfileb.txt"));
            Assert.True(glob.IsMatch("smallfiled.txt"));
            Assert.True(glob.IsMatch("smallfile-.txt"));
            Assert.False(glob.IsMatch("smallfileaa.txt"));
        }

        [Fact]
        public void CanMatchDirectoryWildcardInTopLevelDirectory()
        {
            const string globPattern = @"/**/somefile";
            var glob = new Glob(globPattern);
            Assert.True(glob.IsMatch("/somefile"));
        }
    }
}
