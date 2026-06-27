using System;
using System.Collections.Generic;
using System.IO;
using GlobExpressions.AST;

namespace GlobExpressions;

internal static class PathTraverser
{
    public static IEnumerable<FileSystemInfo> Traverse(this DirectoryInfo root, string pattern, bool caseSensitive, bool emitFiles, bool emitDirectories)
    {
        var parser = new Parser(pattern);
        var segments = parser.ParseTree().Segments;

        var options = new TraverseOptions(caseSensitive, emitFiles, emitDirectories);

        return segments.Length == 0 ? Array.Empty<FileSystemInfo>() : Traverse(root, segments, options);
    }

    internal static IEnumerable<FileSystemInfo> Traverse(DirectoryInfo root, Segment[] segments, TraverseOptions options) =>
        new RecursiveGlobEnumerable(root, segments, options);
}
