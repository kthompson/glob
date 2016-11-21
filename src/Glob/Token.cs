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

        public TokenKind Kind { get; private set; }
        public string Spelling { get; private set; }

        public override string ToString()
        {
            return Kind + ": " + Spelling;
        }
    }
}
