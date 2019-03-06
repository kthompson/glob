using System.Collections.Generic;
using System.IO;

namespace GlobExpressions
{
    internal class TraverseOptions
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

        private readonly FileInfo[] _emptyFileInfos = new FileInfo[0];
        private readonly DirectoryInfo[] _emptyDirectoryInfos = new DirectoryInfo[0];

        public virtual FileInfo[] GetFiles(DirectoryInfo root)
        {
            if (_fileCache.TryGetValue(root.FullName, out var cachedFiles))
                return cachedFiles;

            root.Refresh();
            var files = root.Exists ? root.GetFiles() : _emptyFileInfos;
            _fileCache.Add(root.FullName, files);
            return files;
        }

        public virtual DirectoryInfo[] GetDirectories(DirectoryInfo root)
        {
            if (_dirCache.TryGetValue(root.FullName, out var cachedFiles))
                return cachedFiles;

            root.Refresh();
            var files = root.Exists ? root.GetDirectories() : _emptyDirectoryInfos;
            _dirCache.Add(root.FullName, files);
            return files;
        }
    }
}
