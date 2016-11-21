using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glob
{
    public static class GlobExtensions
    {
        public static IEnumerable<DirectoryInfo> GlobDirectories(this DirectoryInfo di, string pattern)
        {
            var glob = new Glob(pattern, GlobOptions.Compiled);

            return di.EnumerateDirectories("*", SearchOption.AllDirectories).Where(directory => glob.IsMatch(directory.FullName));
        }

        public static IEnumerable<FileInfo> GlobFiles(this DirectoryInfo di, string pattern)
        {
            var glob = new Glob(pattern, GlobOptions.Compiled);

            return di.EnumerateFiles("*", SearchOption.AllDirectories).Where(directory => glob.IsMatch(directory.FullName));
        }

        public static IEnumerable<FileSystemInfo> GlobFileSystemInfos(this DirectoryInfo di, string pattern)
        {
            var glob = new Glob(pattern, GlobOptions.Compiled);

            return di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).Where(directory => glob.IsMatch(directory.FullName));
        }
    }
}
