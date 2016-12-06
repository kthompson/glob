using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glob
{
    class Token
    {
        public Token(TokenKind kind, string spelling)
        {
            this.Kind = kind;
            this.Spelling = spelling;
        }

        public TokenKind Kind { get; }
        public string Spelling { get; }

        public override string ToString()
        {
            return Kind + ": " + Spelling;
        }
    }
}
