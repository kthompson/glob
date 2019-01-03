using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GlobExpressions.AST;

namespace GlobExpressions
{
    public partial class Glob
    {
        public string Pattern { get; }

        private Tree _root;
        private Segment[] _segments;
        private readonly bool _caseSensitive;
        private readonly bool _matchFilenameOnly;

        public Glob(string pattern, GlobOptions options = GlobOptions.None)
        {
            this.Pattern = pattern;
            _caseSensitive = !options.HasFlag(GlobOptions.CaseInsensitive);
            _matchFilenameOnly = !options.HasFlag(GlobOptions.MatchFullPath);

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
            if (_matchFilenameOnly && _segments.Length == 1)
            {
                var last = pathSegments.LastOrDefault();
                var tail = (last == null) ? new string[0] : new[] { last };

                if (GlobEvaluator.Eval(_segments, 0, tail, 0, _caseSensitive))
                    return true;
            }

            return GlobEvaluator.Eval(_segments, 0, pathSegments, 0, _caseSensitive);
        }
    }
}
