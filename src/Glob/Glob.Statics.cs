using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GlobExpressions
{
    public partial class Glob
    {
        public static bool IsMatch(string input, string pattern, GlobOptions options = GlobOptions.None) =>
            new Glob(pattern, options).IsMatch(input);

        public static IEnumerable<string> Files(string workingDirectory, string pattern)
        {
            var directoryInfo = new DirectoryInfo(workingDirectory);

            var truncateLength = GetTruncateLength(directoryInfo);

            return Files(directoryInfo, pattern).Select(info => info.FullName.Remove(0, truncateLength));
        }

        private static int GetTruncateLength(FileSystemInfo directoryInfo) =>
            directoryInfo.FullName.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? directoryInfo.FullName.Length
                : directoryInfo.FullName.Length + 1;

        public static IEnumerable<FileInfo> Files(DirectoryInfo workingDirectory, string pattern)
        {
            var glob = new Glob(pattern, GlobOptions.Compiled);
            var truncateLength = GetTruncateLength(workingDirectory);

            return workingDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(info => glob.IsMatch(info.FullName.Remove(0, truncateLength)));
        }

        public static IEnumerable<string> Directories(string workingDirectory, string pattern)
        {
            var directoryInfo = new DirectoryInfo(workingDirectory);
            var truncateLength = GetTruncateLength(directoryInfo);

            return Directories(directoryInfo, pattern).Select(info => info.FullName.Remove(0, truncateLength));
        }

        public static IEnumerable<DirectoryInfo> Directories(DirectoryInfo workingDirectory, string pattern)
        {
            var glob = new Glob(pattern, GlobOptions.Compiled);
            var truncateLength = GetTruncateLength(workingDirectory);

            return workingDirectory.EnumerateDirectories("*", SearchOption.AllDirectories)
                .Where(info => glob.IsMatch(info.FullName.Remove(0, truncateLength)));
        }

        public static IEnumerable<string> FilesAndDirectories(string workingDirectory, string pattern)
        {
            var directoryInfo = new DirectoryInfo(workingDirectory);
            var truncateLength = GetTruncateLength(directoryInfo);

            return FilesAndDirectories(directoryInfo, pattern).Select(info => info.FullName.Remove(0, truncateLength));
        }

        public static IEnumerable<FileSystemInfo> FilesAndDirectories(DirectoryInfo workingDirectory, string pattern)
        {
            var glob = new Glob(pattern, GlobOptions.Compiled);
            var truncateLength = GetTruncateLength(workingDirectory);

            return workingDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
                .Where(info => glob.IsMatch(info.FullName.Remove(0, truncateLength)));
        }
    }
}
