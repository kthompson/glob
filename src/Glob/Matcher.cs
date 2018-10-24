using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using GlobExpressions.AST;

namespace GlobExpressions
{
    internal static class Matcher
    {
        public static bool MatchesSegment(this DirectorySegment segment, string pathSegment, bool caseSensitive) =>
            MatchesSubSegment(segment.SubSegments, 0, -1, pathSegment, 0, caseSensitive);

        private static bool MatchesSubSegment(SubSegment[] segments, int segmentIndex, int literalSetIndex, string pathSegment, int pathIndex, bool caseSensitive)
        {
            var nextSegment = segmentIndex + 1;
            if (nextSegment > segments.Length)
                return pathIndex == pathSegment.Length;

            var head = segments[segmentIndex];
            if (head is LiteralSet ls)
            {
                if (literalSetIndex == -1)
                {
                    for (int i = 0; i < ls.Literals.Length; i++)
                    {
                        if (MatchesSubSegment(segments, segmentIndex, i, pathSegment, pathIndex, caseSensitive))
                            return true;
                    }

                    return false;
                }

                head = ls.Literals[literalSetIndex];
            }

            switch (head)
            {
                // match zero or more chars
                case StringWildcard _:
                    return MatchesSubSegment(segments, nextSegment, -1, pathSegment, pathIndex, caseSensitive) // zero
                           || (pathIndex < pathSegment.Length &&
                               MatchesSubSegment(segments, segmentIndex, -1, pathSegment, pathIndex + 1, caseSensitive)); // or one+

                case CharacterWildcard _:
                    return pathIndex < pathSegment.Length && MatchesSubSegment(segments, nextSegment, -1, pathSegment, pathIndex + 1, caseSensitive);

                case Identifier ident:
                    var len = ident.Value.Length;
                    if (len + pathIndex > pathSegment.Length)
                        return false;

                    if (!string.Equals(pathSegment.Substring(pathIndex, ident.Value.Length), ident.Value, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        return false;

                    return MatchesSubSegment(segments, nextSegment, -1, pathSegment, pathIndex + len, caseSensitive);

                case CharacterSet set:
                    if (pathIndex == pathSegment.Length)
                        return false;

                    var inThere = set.Matches(pathSegment[pathIndex], caseSensitive);
                    return inThere && MatchesSubSegment(segments, nextSegment, -1, pathSegment, pathIndex + 1, caseSensitive);

                default:
                    return false;
            }
        }
    }
}
