using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            return segments.Length == 0 ? new FileSystemInfo[0] : Traverse(root, segments, 0, cache);
        }

        private static readonly FileSystemInfo[] emptyFileSystemInfoArray = new FileSystemInfo[0];

        private class TraverseOptions
        {
            private readonly Dictionary<string, FileInfo[]> _fileCache = new Dictionary<string, FileInfo[]>();
            private readonly Dictionary<string, DirectoryInfo[]> _dirCache = new Dictionary<string, DirectoryInfo[]>();

            public TraverseOptions(bool caseSensitive, bool emitFiles, bool emitDirectories)
            {
                CaseSensitive = caseSensitive;
                EmitFiles = emitFiles;
                EmitDirectories = emitDirectories;
            }

            public bool CaseSensitive { get; }
            public bool EmitFiles { get; }
            public bool EmitDirectories { get; }

            public FileInfo[] GetFiles(DirectoryInfo root)
            {
                if (_fileCache.TryGetValue(root.FullName, out var cachedFiles))
                    return cachedFiles;

                var files = root.GetFiles();
                _fileCache.Add(root.FullName, files);
                return files;
            }

            public DirectoryInfo[] GetDirectories(DirectoryInfo root)
            {
                if (_dirCache.TryGetValue(root.FullName, out var cachedFiles))
                    return cachedFiles;

                var files = root.GetDirectories();
                _dirCache.Add(root.FullName, files);
                return files;
            }
        }

        private static IEnumerable<FileSystemInfo> Traverse(DirectoryInfo root, Segment[] segments, int segmentIndex,
             TraverseOptions options)
        {
            if (segmentIndex == segments.Length)
            {
                return options.EmitDirectories
                    ? Enumerable.Repeat<FileSystemInfo>(root, 1)
                    : emptyFileSystemInfoArray;
            }

            var segment = segments[segmentIndex];

            switch (segment)
            {
                case DirectorySegment directorySegment:
                    {
                        var filesToEmit =
                            (options.EmitFiles && segmentIndex == segments.Length - 1)
                                ? options.GetFiles(root).Where(file => directorySegment.MatchesSegment(file.Name, options.CaseSensitive)).Cast<FileSystemInfo>()
                                : emptyFileSystemInfoArray;

                        var dirSegmentItems = from directory in options.GetDirectories(root)
                                              where directorySegment.MatchesSegment(directory.Name, options.CaseSensitive)
                                              from item in Traverse(directory, segments, segmentIndex + 1, options)
                                              select item;

                        return filesToEmit.Concat(dirSegmentItems);
                    }
                case DirectoryWildcard _:
                    {
                        var filesToEmit =
                            (options.EmitFiles && segmentIndex == segments.Length - 1)
                                ? options.GetFiles(root).Cast<FileSystemInfo>()
                                : emptyFileSystemInfoArray;

                        // match zero path segments, consuming DirectoryWildcard
                        var zeroMatch = Traverse(root, segments, segmentIndex + 1, options);

                        // match consume 1 path segment but not the Wildcard
                        var files = from directory in options.GetDirectories(root)
                                    from item in Traverse(directory, segments, segmentIndex, options)
                                    select item;

                        return filesToEmit.Concat(zeroMatch).Concat(files);
                    }

                default:
                    return emptyFileSystemInfoArray;
            }
        }
    }
}
