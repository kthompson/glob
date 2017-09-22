using GlobExpressions.AST;

namespace GlobExpressions
{
    internal static class GlobEvaluator
    {
        public static bool Eval(Segment[] pattern, int patternIndex, string[] input, int inputIndex, bool caseSensitive)
        {
            while (true)
            {
                if (inputIndex == input.Length)
                    return patternIndex == pattern.Length;

                // we have a input to match but no pattern to match against so we are done.
                if (patternIndex == pattern.Length)
                    return false;

                var inputHead = input[inputIndex];
                var patternHead = pattern[patternIndex];

                switch (patternHead)
                {
                    case DirectoryWildcard _:
                        // return all consuming the wildcard
                        return Eval(pattern, patternIndex + 1, input, inputIndex, caseSensitive) // 0 matches
                               || Eval(pattern, patternIndex, input, inputIndex + 1, caseSensitive); // 1 or more

                    case Root root when inputHead == root.Text:
                        patternIndex++;
                        inputIndex++;
                        continue;

                    case DirectorySegment dir when dir.MatchesSegment(inputHead, caseSensitive):
                        patternIndex++;
                        inputIndex++;
                        continue;
                }

                return false;
            }
        }
    }
}
