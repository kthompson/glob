using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glob
{
    public class Glob
    {
        private readonly GlobOptions _options;
        public string Pattern { get; private set; }

        private GlobNode _root;
        private Regex _regex;

        public Glob(string pattern, GlobOptions options = GlobOptions.None)
        {
            _options = options;
            this.Pattern = pattern;
            if(options.HasFlag(GlobOptions.Compiled))
            {
                this.Compile();
            }
        }

        private void Compile()
        {
            if(_root != null)
                return;

            if (_regex != null)
                return;

            var parser = new Parser(this.Pattern);
            _root = parser.Parse();

            var regexPattern = GlobToRegexVisitor.Process(_root);

            _regex = new Regex(regexPattern, _options == GlobOptions.Compiled ? RegexOptions.Compiled | RegexOptions.Singleline : RegexOptions.Singleline);
        }

        public bool IsMatch(string input)
        {
            this.Compile();

            return _regex.IsMatch(input);
        }

        public static bool IsMatch(string input, string pattern)
        {
            return new Glob(pattern).IsMatch(input);
        }
    }
}
