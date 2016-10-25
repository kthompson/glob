using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Glob.Tests
{
    public class GlobTests
    {
        [Fact(Skip = "Currently failing")]
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
    }
}
