using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using GlobExpressions.AST;

namespace GlobExpressions;

// Traverses a directory tree with a single recursive FileSystemEnumerable,
// pushing glob matching into the enumeration:
//   ShouldRecursePredicate prunes subtrees that cannot match the pattern,
//   ShouldIncludePredicate performs the full glob match against each entry.
// Only matched entries are materialized into FileSystemInfo objects.
internal sealed class RecursiveGlobEnumerable : IEnumerable<FileSystemInfo>
{
    private static readonly char[] Separators = { '/', '\\' };

    private readonly string _root;
    private readonly Segment[] _segments;
    private readonly bool _caseSensitive;
    private readonly bool _emitFiles;
    private readonly bool _emitDirectories;

    private static readonly EnumerationOptions _enumerationOptions = new EnumerationOptions
    {
        RecurseSubdirectories = true,
        AttributesToSkip = FileAttributes.None,
        IgnoreInaccessible = true,
    };

    public RecursiveGlobEnumerable(DirectoryInfo root, Segment[] segments, TraverseOptions options)
    {
        _root = root.FullName;
        _segments = segments;
        _caseSensitive = options.CaseSensitive;
        _emitFiles = options.EmitFiles;
        _emitDirectories = options.EmitDirectories;
    }

    public IEnumerator<FileSystemInfo> GetEnumerator() => Enumerate().GetEnumerator();

    private IEnumerable<FileSystemInfo> Enumerate()
    {
        if (!Directory.Exists(_root))
            yield break;

        // A pattern made up entirely of "**" segments also matches the search
        // root itself (each ** can match zero directories). FileSystemEnumerable
        // never visits the root, so emit it explicitly to preserve that behavior.
        if (_emitDirectories && MatchesRoot(_segments))
            yield return new DirectoryInfo(_root);

        var segments = _segments;
        var caseSensitive = _caseSensitive;
        var emitFiles = _emitFiles;
        var emitDirectories = _emitDirectories;

        var enumerable = new FileSystemEnumerable<FileSystemInfo>(
            _root,
            static (ref FileSystemEntry entry) => entry.ToFileSystemInfo(),
            _enumerationOptions)
        {
            ShouldIncludePredicate = (ref FileSystemEntry entry) =>
            {
                if (entry.IsDirectory ? !emitDirectories : !emitFiles)
                    return false;

                var relativePath = GetRelativePath(ref entry);
                return PathMatcher.IsFullMatch(segments, relativePath, caseSensitive);
            },
            ShouldRecursePredicate = (ref FileSystemEntry entry) =>
            {
                var relativeDir = GetRelativePath(ref entry);
                return PathMatcher.CanRecurse(segments, relativeDir, caseSensitive);
            },
        };

        foreach (var item in enumerable)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // True when every segment is a "**" wildcard, in which case the pattern also
    // matches the traversal root (zero directories deep).
    private static bool MatchesRoot(Segment[] segments)
    {
        foreach (var segment in segments)
        {
            if (segment is not DirectoryWildcard)
                return false;
        }

        return true;
    }

    // Builds the entry's path relative to the traversal root, e.g. "a/b/file.txt".
    private static string GetRelativePath(ref FileSystemEntry entry)
    {
        var root = entry.RootDirectory;
        var dir = entry.Directory;

        // Portion of the entry's parent directory below the traversal root.
        ReadOnlySpan<char> relativeParent = dir.Length > root.Length
            ? dir.Slice(root.Length)
            : ReadOnlySpan<char>.Empty;
        relativeParent = relativeParent.TrimStart(Separators);

        var fileName = entry.FileName;
        if (relativeParent.IsEmpty)
            return fileName.ToString();

        return string.Concat(relativeParent, "/".AsSpan(), fileName);
    }
}
