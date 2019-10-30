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

        public static IEnumerable<string> Files(string workingDirectory, string pattern, GlobOptions options)
        {
            var directoryInfo = new DirectoryInfo(workingDirectory);

            var truncateLength = GetTruncateLength(directoryInfo);

            return Files(directoryInfo, pattern, options).Select(RemovePrefix<FileInfo>(truncateLength));
        }

        public static IEnumerable<string> Files(string workingDirectory, string pattern) =>
            Files(workingDirectory, pattern, GlobOptions.Compiled);

        public static IEnumerable<FileInfo> Files(DirectoryInfo workingDirectory, string pattern) =>
            Files(workingDirectory, pattern, GlobOptions.Compiled);

        public static IEnumerable<FileInfo> Files(DirectoryInfo workingDirectory, string pattern, GlobOptions options) =>
            workingDirectory
                .Traverse(pattern, !options.HasFlag(GlobOptions.CaseInsensitive), true, false)
                .OfType<FileInfo>();

        public static IEnumerable<string> Directories(string workingDirectory, string pattern) =>
            Directories(workingDirectory, pattern, GlobOptions.Compiled);

        public static IEnumerable<string> Directories(string workingDirectory, string pattern, GlobOptions options)
        {
            var directoryInfo = new DirectoryInfo(workingDirectory);
            var truncateLength = GetTruncateLength(directoryInfo);

            return Directories(directoryInfo, pattern, options).Select(RemovePrefix<DirectoryInfo>(truncateLength));
        }

        private static Func<T, string> RemovePrefix<T>(int truncateLength) where T : FileSystemInfo
        {
            return info =>
            {
                // For cases when the pattern matches the root entry, return an empty string
                if (info.FullName.Length <= truncateLength)
                    return "";

                return info.FullName.Remove(0, truncateLength);
            };
        }

        public static IEnumerable<DirectoryInfo> Directories(DirectoryInfo workingDirectory, string pattern) =>
            Directories(workingDirectory, pattern, GlobOptions.Compiled);

        public static IEnumerable<DirectoryInfo> Directories(DirectoryInfo workingDirectory, string pattern, GlobOptions options) =>
            workingDirectory
                .Traverse(pattern, !options.HasFlag(GlobOptions.CaseInsensitive), false, true)
                .OfType<DirectoryInfo>();

        public static IEnumerable<string> FilesAndDirectories(string workingDirectory, string pattern) =>
            FilesAndDirectories(workingDirectory, pattern, GlobOptions.Compiled);

        public static IEnumerable<string> FilesAndDirectories(string workingDirectory, string pattern, GlobOptions options)
        {
            var directoryInfo = new DirectoryInfo(workingDirectory);
            var truncateLength = GetTruncateLength(directoryInfo);

            return FilesAndDirectories(directoryInfo, pattern, options).Select(RemovePrefix<FileSystemInfo>(truncateLength));
        }

        public static IEnumerable<FileSystemInfo> FilesAndDirectories(DirectoryInfo workingDirectory, string pattern) =>
            FilesAndDirectories(workingDirectory, pattern, GlobOptions.Compiled);

        public static IEnumerable<FileSystemInfo> FilesAndDirectories(DirectoryInfo workingDirectory, string pattern, GlobOptions options) =>
            workingDirectory.Traverse(pattern, !options.HasFlag(GlobOptions.CaseInsensitive), true, true);

        private static int GetTruncateLength(FileSystemInfo directoryInfo)
        {
            Console.WriteLine(Path.DirectorySeparatorChar);
            Console.WriteLine(directoryInfo.FullName.EndsWith(Path.DirectorySeparatorChar.ToString()));
            return directoryInfo.FullName.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? directoryInfo.FullName.Length
                : directoryInfo.FullName.Length + 1;
        }
    }
}
