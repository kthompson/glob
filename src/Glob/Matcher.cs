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
            MatchesSubSegment(segment.SubSegments, 0, pathSegment, 0, caseSensitive);

        private static bool MatchesSubSegment(SubSegment[] segments, int segmentIndex, string pathSegment, int pathIndex, bool caseSensitive)
        {
            var nextSegment = segmentIndex + 1;
            if (nextSegment > segments.Length)
                return pathIndex == pathSegment.Length;

            var head = segments[segmentIndex];

            switch (head)
            {
                // match zero or more chars
                case StringWildcard _:
                    return MatchesSubSegment(segments, segmentIndex + 1, pathSegment, pathIndex, caseSensitive) // zero
                           || (pathIndex < pathSegment.Length &&
                               MatchesSubSegment(segments, segmentIndex, pathSegment, pathIndex + 1, caseSensitive)); // or one+

                case CharacterWildcard _:
                    return pathIndex < pathSegment.Length && MatchesSubSegment(segments, nextSegment, pathSegment, pathIndex + 1, caseSensitive);

                case Identifier ident:
                    var len = ident.Value.Length;
                    if (len + pathIndex > pathSegment.Length)
                        return false;

                    if (!string.Equals(pathSegment.Substring(pathIndex, ident.Value.Length), ident.Value, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        return false;

                    return MatchesSubSegment(segments, nextSegment, pathSegment, pathIndex + len, caseSensitive);

                case LiteralSet literalSet:
                    //TODO we can probably optimize this somehow to get rid of the allocations...
                    var tail = segments.Skip(nextSegment).ToArray();
                    return literalSet.Literals.Any(lit => MatchesSubSegment(new SubSegment[] { lit }.Concat(tail).ToArray(), 0, pathSegment, pathIndex, caseSensitive));

                case CharacterSet set:
                    if (pathIndex == pathSegment.Length)
                        return false;

                    var inThere = set.Matches(pathSegment[pathIndex], caseSensitive);
                    return inThere && MatchesSubSegment(segments, nextSegment, pathSegment, pathIndex + 1, caseSensitive);
            }
            return false;
        }
    }
}
