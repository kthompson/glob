using System;
using System.Collections;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
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
        return fileSystem.DirectoryInfo.FromDirectoryName(root.FullName).GetFiles().Select(x => new FileInfo(x.FullName)).ToArray();
    }

    public override DirectoryInfo[] GetDirectories(DirectoryInfo root)
    {
        return fileSystem.DirectoryInfo.FromDirectoryName(root.FullName).GetDirectories().Select(x => new DirectoryInfo(x.FullName)).ToArray();
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