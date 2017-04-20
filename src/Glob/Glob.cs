using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glob
{
    public class Glob
    {
        public string Pattern { get; }

        private Tree _root;
        private Segment[] _segments;


        public Glob(string pattern, GlobOptions options = GlobOptions.None)
        {
            this.Pattern = pattern;
            if (options.HasFlag(GlobOptions.Compiled))
            {
                this.Compile();
            }
        }

        private void Compile()
        {
            if (_root != null)
                return;

            if (_segments != null)
                return;

            var parser = new Parser(this.Pattern);
            _root = parser.ParseTree();
            _segments = _root.Segments;
        }

        public bool IsMatch(string input)
        {
            this.Compile();

            var pathSegments = input.Split('/', '\\');
            // match filename only
            if (_segments.Length == 1)
            {
                var last = pathSegments.LastOrDefault();
                var tail = (last == null) ? new string[0] : new[] {last};

                if (IsMatch(_segments, 0, tail, 0))
                    return true;
            }

            return IsMatch(_segments, 0, pathSegments, 0);
        }

        static bool IsMatch(Segment[] pattern, int patternIndex, string[] input, int inputIndex)
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
                        return IsMatch(pattern, patternIndex + 1, input, inputIndex) // 0 matches
                               || IsMatch(pattern, patternIndex + 1, input, inputIndex + 1) // 1 match
                               || IsMatch(pattern, patternIndex, input, inputIndex + 1); // more

                    case Root root when inputHead == root.Text:
                        patternIndex++;
                        inputIndex++;
                        continue;

                    case DirectorySegment dir when dir.MatchesSegment(inputHead):
                        patternIndex++;
                        inputIndex++;
                        continue;
                }

                return false;

            }
        }

        public static bool IsMatch(string input, string pattern, GlobOptions options = GlobOptions.None) =>
            new Glob(pattern, options).IsMatch(input);

    }
}
