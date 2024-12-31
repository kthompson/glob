using System;
using System.Collections;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace GlobExpressions.Tests;

internal class MockTraverseOptions : TraverseOptions, IEnumerable
{
    private readonly IFileSystem fileSystem;

    public MockTraverseOptions(bool caseSensitive, bool emitFiles, bool emitDirectories, IFileSystem fileSystem)
        : base(caseSensitive, emitFiles, emitDirectories)
    {
        this.fileSystem = fileSystem;
    }

    internal readonly MockFileSystemNode _root = new MockFileSystemNode("", null);

    public override FileInfo[] GetFiles(DirectoryInfo root)
    {
        var directoryInfo = fileSystem.DirectoryInfo.New(root.FullName);

        return directoryInfo
            .GetFiles()
            .Select(file => new FileInfo(file.FullName))
            .ToArray();
    }

    public override DirectoryInfo[] GetDirectories(DirectoryInfo root)
    {
        var directoryInfo = fileSystem.DirectoryInfo.New(root.FullName);

        return directoryInfo
            .GetDirectories()
            .Select(dir => new DirectoryInfo(dir.FullName))
            .ToArray();
    }


    // fake enumerator
    public IEnumerator GetEnumerator()
    {
        return Array.Empty<int>().GetEnumerator();
    }

    public void Add(string path)
    {
        _root.Add(path);
    }
}
