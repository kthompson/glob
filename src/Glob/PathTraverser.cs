using System;
using System.Collections;
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

        private static readonly FileSystemInfo[] _emptyFileSystemInfoArray = new FileSystemInfo[0];
        private static readonly DirectoryInfo[] _emptyPathJobArray = new DirectoryInfo[0];

        internal static IEnumerable<FileSystemInfo> Traverse(DirectoryInfo root, Segment[] segments, int segmentIndex,
            TraverseOptions options) =>
            Traverse(new List<DirectoryInfo> { root }, segments, options);

        private static IEnumerable<FileSystemInfo> Traverse(List<DirectoryInfo> roots, Segment[] segments, TraverseOptions options)
        {
            var segmentsLength = segments.Length;
            var rootCache = new List<DirectoryInfo>();
            var nextSegmentRoots = new List<DirectoryInfo>();
            var segmentIndex = 0;
            var emitDirectories = options.EmitDirectories;
            var emitFiles = options.EmitFiles;

            void Swap(ref List<DirectoryInfo> other)
            {
                var swap = roots;
                roots = other;
                other = swap;
            }

            IEnumerable<DirectoryInfo> JobsMatchingSegment(DirectoryInfo directoryInfo, Segment segment)
            {
                switch (segment)
                {
                    case DirectorySegment directorySegment:
                        // consume DirectorySegment
                        var pathJobs = (from directory in options.GetDirectories(directoryInfo)
                                        where directorySegment.MatchesSegment(directory.Name, options.CaseSensitive)
                                        select directory).ToArray();

                        nextSegmentRoots.AddRange(pathJobs);

                        return _emptyPathJobArray;

                    case DirectoryWildcard _:
                        {
                            // match zero path segments, consuming DirectoryWildcard
                            nextSegmentRoots.Add(directoryInfo);

                            // match consume 1 path segment but not the Wildcard
                            return options.GetDirectories(directoryInfo);
                        }

                    default:
                        return _emptyPathJobArray;
                }
            }

            while (true)
            {
                // no more segments. return all current roots
                var noMoreSegments = segmentIndex == segmentsLength;
                if (emitDirectories && noMoreSegments)
                {
                    foreach (var info in roots)
                    {
                        yield return info;
                    }
                }

                // no more roots or no more segments, go to next segment
                if (roots.Count == 0 || noMoreSegments)
                {
                    roots.Clear();
                    if (nextSegmentRoots.Count > 0)
                    {
                        Swap(ref nextSegmentRoots);
                        segmentIndex++;
                        continue;
                    }

                    yield break;
                }

                var segment = segments[segmentIndex];
                var onLastSegment = segmentIndex == segmentsLength - 1;
                if (emitFiles && onLastSegment)
                {
                    var allFiles = from job in roots
                                   let children = options.GetFiles(job)
                                   from file in FilesMatchingSegment(children, segment, options.CaseSensitive)
                                   select file;

                    foreach (var info in allFiles)
                    {
                        yield return info;
                    }
                }
                rootCache.Clear();
                rootCache.AddRange(roots.SelectMany(job => JobsMatchingSegment(job, segment)));

                Swap(ref rootCache);
            }
        }

        private static IEnumerable<FileSystemInfo> FilesMatchingSegment(IEnumerable<FileInfo> fileInfos, Segment segment, bool caseSensitive) =>
            segment switch
            {
                DirectorySegment directorySegment => (IEnumerable<FileSystemInfo>)fileInfos.Where(file => directorySegment.MatchesSegment(file.Name, caseSensitive)),
                DirectoryWildcard _ => fileInfos,
                _ => _emptyFileSystemInfoArray
            };
    }
}
