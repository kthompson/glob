using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace GlobExpressions.Tests
{
    public class MockTraverseOptionsTests
    {
        [Fact]
        public void CanAddPaths()
        {
            var options = new MockTraverseOptions(false, false, false, new MockFileSystem(new Dictionary<string, MockFileData>
            {
                ["c:/Windows/Notepad.exe"] = MockFileData.NullObject,
                ["c:/Windows/explorer.exe"] = MockFileData.NullObject,
                ["c:/Windows/System32/at.exe"] = MockFileData.NullObject,
            }));

            var directories = options.GetDirectories(new DirectoryInfo("C:\\"));
            Assert.Collection(directories, info => Assert.Equal("c:\\Windows", info.FullName));

            var directories2 = options.GetDirectories(new DirectoryInfo("C:\\Windows"));
            Assert.Collection(directories2, info => Assert.Equal("c:\\Windows\\System32", info.FullName));

            var files = options.GetFiles(new DirectoryInfo("C:\\Windows"));
            Assert.Collection(files,
                info => Assert.Equal("c:\\Windows\\Notepad.exe", info.FullName),
                info => Assert.Equal("c:\\Windows\\explorer.exe", info.FullName)
            );
        }

        [Fact]
        public void CtorPassesCaseSensitive()
        {
            Assert.False(new MockTraverseOptions(false, false, false, new MockFileSystem()).CaseSensitive);
            Assert.True(new MockTraverseOptions(true, false, false, new MockFileSystem()).CaseSensitive);
        }

        [Fact]
        public void CtorPassesEmitFiles()
        {
            Assert.False(new MockTraverseOptions(false, false, false, new MockFileSystem()).EmitFiles);
            Assert.True(new MockTraverseOptions(false, true, false, new MockFileSystem()).EmitFiles);
        }

        [Fact]
        public void CtorPassesEmitDirectories()
        {
            Assert.False(new MockTraverseOptions(false, false, false, new MockFileSystem()).EmitDirectories);
            Assert.True(new MockTraverseOptions(false, false, true, new MockFileSystem()).EmitDirectories);
        }
    }
}
