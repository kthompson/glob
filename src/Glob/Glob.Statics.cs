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

        public static IEnumerable<string> Files(string pattern)
        {
            var workingDirectory = "";
            var subpattern = "";
            var openingNonLiterals = "*?[{";
            for (var i = 0; i < pattern.Length; i++)
            {
                if (openingNonLiterals.Contains(pattern[i]) && (i > 0) && (pattern[i - 1] != '\\')) // find first non-escaped non-literal character
                {
                    workingDirectory = pattern.Substring(0, i);
                    subpattern = pattern.Substring(i);
                    break;
                }
            }

            if (workingDirectory.EndsWith("/"))
            {
                return Files(workingDirectory, subpattern, GlobOptions.Compiled);
            }
            else
            {
                var lastId = workingDirectory.LastIndexOf('/') + 1;
                subpattern = workingDirectory.Substring(lastId) + subpattern;
                workingDirectory = workingDirectory.Substring(0, lastId);
                return Files(workingDirectory, subpattern, GlobOptions.Compiled);
            }
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

        private static int GetTruncateLength(FileSystemInfo directoryInfo) =>
            directoryInfo.FullName.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? directoryInfo.FullName.Length
                : directoryInfo.FullName.Length + 1;
    }
}
