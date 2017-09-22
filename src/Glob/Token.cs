using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobExpressions
{
    internal struct Token
    {
        private readonly string _source;
        private readonly int _startIndex;
        private readonly int _length;

        public Token(TokenKind kind, string source, int startIndex, int length)
        {
            _source = source;
            _startIndex = startIndex;
            _length = length;
            this.Kind = kind;
        }

        public TokenKind Kind { get; }
        public string Spelling => _source.Substring(_startIndex, _length);

        public override string ToString()
        {
            return Kind + ": " + Spelling;
        }
    }
}
