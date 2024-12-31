using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;
using static GlobExpressions.Tests.TestHelpers;

namespace GlobExpressions.Tests;

public class MockTraverseOptionsTests
{

    [Fact]
    public void CanAddPaths()
    {
        var windowsPath = Path.Combine(FileSystemRoot, "Windows");
        var system32Path = Path.Combine(windowsPath, "System32");

        var mockFiles = new Dictionary<string, MockFileData>
        {
            [Path.Combine(windowsPath, "Notepad.exe")] = new MockFileData(string.Empty),
            [Path.Combine(windowsPath, "explorer.exe")] = new MockFileData(string.Empty),
            [Path.Combine(system32Path, "at.exe")] = new MockFileData(string.Empty),
        };
        var mockFileSystem = new MockFileSystem(mockFiles);
        var options = new MockTraverseOptions(false, false, false, mockFileSystem);

        var directories = options.GetDirectories(new DirectoryInfo(FileSystemRoot))
            .Where(info => info.FullName == windowsPath)
            .ToArray();

        Assert.Collection(directories, info => Assert.Equal(windowsPath, info.FullName));

        var directories2 = options.GetDirectories(new DirectoryInfo(windowsPath))
            .Where(info => info.FullName == system32Path)
            .ToArray();

        Assert.Collection(directories2, info => Assert.Equal(system32Path, info.FullName));

        var files = options.GetFiles(new DirectoryInfo(windowsPath));
        Assert.Collection(files,
            info => Assert.Equal(Path.Combine(windowsPath, "Notepad.exe"), info.FullName),
            info => Assert.Equal(Path.Combine(windowsPath, "explorer.exe"), info.FullName)
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
