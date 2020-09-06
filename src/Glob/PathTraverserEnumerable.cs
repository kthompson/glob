using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobExpressions.AST;

namespace GlobExpressions
{
    internal class PathTraverserEnumerable : IEnumerable<FileSystemInfo>
    {
        private readonly List<DirectoryInfo> _originalRoots;
        private readonly Segment[] _segments;
        private readonly TraverseOptions _options;

        private static readonly FileSystemInfo[] _emptyFileSystemInfoArray = new FileSystemInfo[0];
        private static readonly DirectoryInfo[] _emptyPathJobArray = new DirectoryInfo[0];
        public PathTraverserEnumerable(List<DirectoryInfo> roots, Segment[] segments, TraverseOptions options)
        {
            this._originalRoots = roots;
            this._segments = segments;
            this._options = options;
        }

        public IEnumerator<FileSystemInfo> GetEnumerator()
        {
            var roots = new List<DirectoryInfo>();
            var rootCache = new List<DirectoryInfo>();
            roots.AddRange(_originalRoots);

            var segmentsLength = _segments.Length;
            var nextSegmentRoots = new List<DirectoryInfo>();
            var segmentIndex = 0;
            var emitDirectories = _options.EmitDirectories;
            var emitFiles = _options.EmitFiles;
            var cache = new HashSet<string>();

            void Swap(ref List<DirectoryInfo> other)
            {
                var swap = roots;
                roots = other;
                other = swap;
            }

            while (true)
            {
                // no more segments. return all current roots
                var noMoreSegments = segmentIndex == segmentsLength;
                if (emitDirectories && noMoreSegments)
                {
                    foreach (var info in roots.Where(info => !cache.Contains(info.FullName)))
                    {
                        cache.Add(info.FullName);
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

                var segment = _segments[segmentIndex];
                var onLastSegment = segmentIndex == segmentsLength - 1;
                if (emitFiles && onLastSegment)
                {
                    var allFiles = from job in roots
                        let children = _options.GetFiles(job)
                        from file in FilesMatchingSegment(children, segment, _options.CaseSensitive)
                        select file;

                    foreach (var info in allFiles)
                    {
                        if (!cache.Contains(info.FullName))
                        {
                            cache.Add(info.FullName);
                            yield return info;
                        }
                    }
                }
                rootCache.Clear();
                rootCache.AddRange(roots.SelectMany(job => JobsMatchingSegment(nextSegmentRoots, job, segment)));

                Swap(ref rootCache);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<DirectoryInfo> JobsMatchingSegment(List<DirectoryInfo> nextSegmentRoots, DirectoryInfo directoryInfo, Segment segment)
        {
            switch (segment)
            {
                case DirectorySegment directorySegment:
                    // consume DirectorySegment
                    var pathJobs = (from directory in _options.GetDirectories(directoryInfo)
                        where directorySegment.MatchesSegment(directory.Name, _options.CaseSensitive)
                        select directory).ToArray();

                    nextSegmentRoots.AddRange(pathJobs);

                    return _emptyPathJobArray;

                case DirectoryWildcard _:
                {
                    // match zero path segments, consuming DirectoryWildcard
                    nextSegmentRoots.Add(directoryInfo);

                    // match consume 1 path segment but not the Wildcard
                    return _options.GetDirectories(directoryInfo);
                }

                default:
                    return _emptyPathJobArray;
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
