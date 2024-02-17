using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using GlobExpressions.AST;

namespace GlobExpressions
{
    internal static class PathTraverser
    {
        public static IEnumerable<FileSystemInfo> Traverse(this DirectoryInfo root, string pattern, bool caseSensitive, bool emitFiles, bool emitDirectories)
        {
            var parser = new Parser(pattern);
            var segments = parser.ParseTree().Segments;

            var cache = new TraverseOptions(caseSensitive, emitFiles, emitDirectories);

            return segments.Length == 0 ? Array.Empty<FileSystemInfo>() : Traverse(root, segments, cache);
        }

        internal static IEnumerable<FileSystemInfo> Traverse(DirectoryInfo root, Segment[] segments, TraverseOptions options) =>
            Traverse(new List<DirectoryInfo> { root }, segments, options);

        private static IEnumerable<FileSystemInfo> Traverse(List<DirectoryInfo> roots, Segment[] segments, TraverseOptions options) =>
            new PathTraverserEnumerable(roots, segments, options);
    }
}
