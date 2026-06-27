using System;
using GlobExpressions.AST;

namespace GlobExpressions;

// Span-based glob matching used by the recursive FileSystemEnumerable traversal.
// Matches an entry's path (relative to the traversal root) against the parsed
// segment array without splitting the path into a string[].
internal static class PathMatcher
{
    private static readonly char[] Separators = { '/', '\\' };

    // Full match: does the relative path match the whole segment pattern?
    public static bool IsFullMatch(Segment[] segments, ReadOnlySpan<char> relativePath, bool caseSensitive) =>
        Eval(segments, 0, relativePath, caseSensitive);

    // Prefix match: could any descendant of the directory at relativeDir match?
    // Used for ShouldRecursePredicate to prune subtrees that cannot match.
    public static bool CanRecurse(Segment[] segments, ReadOnlySpan<char> relativeDir, bool caseSensitive)
    {
        var si = 0;
        while (true)
        {
            // Pattern fully consumed with no trailing wildcard: a deeper path
            // would be longer than the pattern, so nothing below can match.
            if (si == segments.Length)
                return false;

            // Prefix matched everything so far and segments remain, so a
            // descendant may complete the match.
            if (relativeDir.IsEmpty)
                return true;

            SplitHead(relativeDir, out var head, out var rest);

            switch (segments[si])
            {
                case DirectoryWildcard _:
                    // ** can absorb arbitrary depth, so always worth recursing.
                    return true;

                case DirectorySegment dir:
                    if (!dir.MatchesSegment(head, caseSensitive))
                        return false;
                    si++;
                    relativeDir = rest;
                    continue;

                default:
                    return false;
            }
        }
    }

    private static bool Eval(Segment[] segments, int si, ReadOnlySpan<char> path, bool caseSensitive)
    {
        while (true)
        {
            if (si == segments.Length)
                return path.IsEmpty;

            // Segments remain but the path is exhausted: no match.
            if (path.IsEmpty)
                return false;

            SplitHead(path, out var head, out var rest);

            switch (segments[si])
            {
                case DirectoryWildcard _:
                    var isLastSegment = si == segments.Length - 1;

                    // ** as the final segment matches everything remaining.
                    if (isLastSegment)
                        return true;

                    // Match zero path components against **.
                    if (Eval(segments, si + 1, path, caseSensitive))
                        return true;

                    // Match one component and keep the ** active.
                    path = rest;
                    continue;

                case DirectorySegment dir:
                    if (!dir.MatchesSegment(head, caseSensitive))
                        return false;
                    si++;
                    path = rest;
                    continue;

                default:
                    return false;
            }
        }
    }

    private static void SplitHead(ReadOnlySpan<char> path, out ReadOnlySpan<char> head, out ReadOnlySpan<char> rest)
    {
        var sep = path.IndexOfAny(Separators[0], Separators[1]);
        if (sep < 0)
        {
            head = path;
            rest = ReadOnlySpan<char>.Empty;
        }
        else
        {
            head = path.Slice(0, sep);
            rest = path.Slice(sep + 1);
        }
    }
}
