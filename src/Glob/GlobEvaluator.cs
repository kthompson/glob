using GlobExpressions.AST;

namespace GlobExpressions
{
    internal static class GlobEvaluator
    {
        public static bool Eval(Segment[] segments, int segmentIndex, string[] input, int inputIndex, bool caseSensitive)
        {
            while (true)
            {
                // no segments left
                if (segmentIndex == segments.Length)
                    return inputIndex == input.Length;

                switch (segments[segmentIndex])
                {
                    case DirectoryWildcard _:
                        var consumedAllInput = inputIndex >= input.Length;
                        var isLastInput = inputIndex == input.Length - 1;
                        var isLastSegment = segmentIndex == segments.Length - 1;

                        // simple match last input and segment
                        if (isLastSegment && (isLastInput || consumedAllInput))
                            return true;

                        // match 0
                        var matchConsumesWildCard = !isLastSegment && Eval(segments, segmentIndex + 1, input, inputIndex, caseSensitive);
                        if (matchConsumesWildCard)
                            return true;

                        // match 1+
                        var skipInput = !isLastInput && Eval(segments, segmentIndex, input, inputIndex + 1, caseSensitive);

                        return skipInput;

                    case Root root:
                        if (inputIndex < input.Length && input[inputIndex] == root.Text)
                        {
                            segmentIndex++;
                            inputIndex++;
                            continue;
                        }
                        else
                        {
                            return false;
                        }

                    case DirectorySegment dir:
                        if (inputIndex < input.Length && dir.MatchesSegment(input[inputIndex], caseSensitive))
                        {
                            segmentIndex++;
                            inputIndex++;
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                }

                return false;
            }
        }
    }
}
