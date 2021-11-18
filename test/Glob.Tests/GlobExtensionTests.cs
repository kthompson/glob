using System;
using System.IO;
using System.Linq;
using Xunit;
using static GlobExpressions.Tests.TestHelpers;

namespace GlobExpressions.Tests
{
    public class GlobExtensionTests
    {
        [Fact]
        public void CanMatchBinFolderGlob()
        {
            var root = new DirectoryInfo(SourceRoot);
            var allBinFolders = root.GlobDirectories("**/bin");

            Assert.True(allBinFolders.Any(), "There should be some bin folders");
        }

        [Fact]
        public void CanMatchBinFolderGlobCaseInsensitive()
        {
            var root = new DirectoryInfo(SourceRoot);
            var allBinFolders = root.GlobDirectories("**/BIN", GlobOptions.CaseInsensitive);

            Assert.True(allBinFolders.Any(), "There should be some BIN folders");
        }

        [Fact]
        public void CanMatchDllExtension()
        {
            var root = new DirectoryInfo(SourceRoot);
            var allDllFiles = root.GlobFiles("**/*.dll");

            Assert.True(allDllFiles.Any(), "There should be some DLL files");
        }

        [Fact]
        public void CanMatchDllExtensionCaseInsensitive()
        {
            var root = new DirectoryInfo(SourceRoot);
            var allDllFiles = root.GlobFiles("**/*.DLL", GlobOptions.CaseInsensitive);

            Assert.True(allDllFiles.Any(), "There should be some DLL files");
        }

        [Fact]
        public void CanMatchInfoInFileSystemInfo()
        {
            var root = new DirectoryInfo(SourceRoot);
            var allInfoFilesAndFolders = root.GlobFileSystemInfos("**/*info");

            Assert.True(allInfoFilesAndFolders.Any(), "There should be some 'allInfoFilesAndFolders'");
        }

        [Fact]
        public void CanMatchInfoInFileSystemInfoCaseInsensitive()
        {
            var root = new DirectoryInfo(SourceRoot);
            var allInfoFilesAndFolders = root.GlobFileSystemInfos("**/*INFO", GlobOptions.CaseInsensitive);

            Assert.True(allInfoFilesAndFolders.Any(), "There should be some 'allINFOFilesAndFolders'");
        }

                [Fact]
        public void CanMatchConfigFilesInMsDirectory()
        {
            var globPattern = @"**/*.sln";

            var root = new DirectoryInfo(SourceRoot);
            var result = root.GlobFiles(globPattern).ToList();

            Assert.NotNull(result);
            Assert.True(result.Any(x => x.Name == "Glob.sln"), $"There should be some Glob.sln files in '{root.FullName}'");
        }

        [Fact]
        public void CanMatchStarThenPath()
        {
            var globPattern = @"*/*/*.csproj";

            var root = new DirectoryInfo(SourceRoot);
            var result = root.GlobFiles(globPattern).OrderBy(x => x.Name.ToLower()).ToList();

            Assert.Collection(
                result,
                file => Assert.Equal("Glob.Benchmarks.csproj", file.Name),
                file => Assert.Equal("Glob.csproj", file.Name),
                file => Assert.Equal("Glob.Tests.csproj", file.Name),
                file => Assert.Equal("GlobApp.csproj", file.Name)
            );
        }

        [Fact]
        public void CanMatchConfigFilesOrFoldersInMsDirectory()
        {
            var globPattern = @"**/*[Tt]est*";

            var root = new DirectoryInfo(SourceRoot);
            var result = root.GlobFileSystemInfos(globPattern).ToList();

            Assert.NotNull(result);
            Assert.True(result.Any(x => x.Name == "GlobTests.cs"), $"There should be some GlobTests.cs files in '{root.FullName}'");
            Assert.True(result.Any(x => x.Name == "test"), $"There should some folder with 'test' in '{root.FullName}'");
        }

        [Fact]
        public void CanMatchDirectoriesInMsDirectory()
        {
            var globPattern = @"**/*Gl*.Te*";

            var root = new DirectoryInfo(SourceRoot);
            var result = root.GlobDirectories(globPattern).ToList();

            Assert.NotNull(result);
            Assert.True(result.Any(), $"There should be some directories that match glob: {globPattern} in '{root.FullName}'");
        }

        [Fact]
        public void CanMatchFilesInDirectoriesWithTrailingSlash()
        {
            var globPattern = @"test/**/*Gl*.Te*";
            var root = new DirectoryInfo(SourceRoot + Path.DirectorySeparatorChar).FullName;
            var result = Glob.Files(root, globPattern).ToList();

            Assert.NotNull(result);
            Assert.All(result, path => Assert.StartsWith("test", path));
            Assert.True(result.Any(), $"There should be some directories that match glob: {globPattern} in '{root}'");
        }

        [Fact]
        public void CanMatchFilesInDirectoriesWithoutTrailingSlash()
        {
            var globPattern = @"test/**/*Gl*.Te*";
            var root = new DirectoryInfo(SourceRoot).FullName;
            var result = Glob.Files(root, globPattern).ToList();

            Assert.NotNull(result);
            Assert.All(result, path => Assert.StartsWith("test", path));
            Assert.True(result.Any(), $"There should be some directories that match glob: {globPattern} in '{root}'");
        }
    }
}
