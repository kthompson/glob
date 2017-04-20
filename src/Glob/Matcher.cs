using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Glob
{
    static class Matcher
    {
        public static bool MatchesSegment(this DirectorySegment segment, string pathSegment) =>
            MatchesSubSegment(segment.SubSegments, 0, pathSegment, 0);

        static bool MatchesSubSegment(SubSegment[] segments, int segmentIndex, string pathSegment, int pathIndex)
        {
            var nextSegment = segmentIndex + 1;
            if (nextSegment > segments.Length)
                return pathIndex == pathSegment.Length;

            var head = segments[segmentIndex];

            switch (head)
            {
                // match zero or more chars
                case StringWildcard _:
                    return MatchesSubSegment(segments, segmentIndex + 1, pathSegment, pathIndex) // zero
                           || (pathIndex < pathSegment.Length &&
                               MatchesSubSegment(segments, segmentIndex, pathSegment, pathIndex + 1)); // or one+

                case CharacterWildcard _:
                    return pathIndex < pathSegment.Length && MatchesSubSegment(segments, nextSegment, pathSegment, pathIndex + 1);

                case Identifier ident:
                    var len = ident.Value.Length;
                    if (len + pathIndex > pathSegment.Length)
                        return false;

                    if (pathSegment.Substring(pathIndex, ident.Value.Length) != ident.Value)
                        return false;

                    return MatchesSubSegment(segments, nextSegment, pathSegment, pathIndex + len);

                case LiteralSet literalSet:
                    //TODO we can probably optimize this somehow to get rid of the allocations...
                    var tail = segments.Skip(nextSegment).ToArray();
                    return literalSet.Literals.Any(lit => MatchesSubSegment(new SubSegment[] { lit }.Concat(tail).ToArray(), 0, pathSegment, pathIndex));

                case CharacterSet set:
                    if (pathIndex == pathSegment.Length)
                        return false;

                    var inThere = set.Matches(pathSegment[pathIndex]);
                    return inThere && MatchesSubSegment(segments, nextSegment, pathSegment, pathIndex + 1);
            }
            return false;

        }
    }
}
