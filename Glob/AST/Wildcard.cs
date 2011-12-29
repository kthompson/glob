using System.IO;

namespace Glob.AST
{
    class Wildcard : SubSegment
    {
        public TokenKind Kind { get; set; }

        public Wildcard(TokenKind kind)
        {
            this.Kind = kind;
        }

        public override string ToGlobString()
        {
            switch (this.Kind)
            {
                case TokenKind.Wildcard:
                    return "*";
                case TokenKind.CharacterWildcard:
                    return "?";
                default:
                    throw new InvalidDataException();
            }
        }

        public override bool IsWildcard
        {
            get { return true; }
        }

        public override string ToRegexString()
        {
            switch (this.Kind)
            {
                case TokenKind.Wildcard:
                    return ".*";
                case TokenKind.CharacterWildcard:
                    return ".{1}";
                default:
                    throw new InvalidDataException();
            }
        }
    }
}