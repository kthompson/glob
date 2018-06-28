using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GlobExpressions
{
    public static class GlobExtensions
    {
        public static IEnumerable<DirectoryInfo> GlobDirectories(this DirectoryInfo di, string pattern)
        {
            return Glob.Directories(di, pattern);
        }

        public static IEnumerable<FileInfo> GlobFiles(this DirectoryInfo di, string pattern)
        {
            return Glob.Files(di, pattern);
        }

        public static IEnumerable<FileSystemInfo> GlobFileSystemInfos(this DirectoryInfo di, string pattern)
        {
            return Glob.FilesAndDirectories(di, pattern);
        }
    }
}
