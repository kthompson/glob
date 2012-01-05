using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glob
{
    class Parser
    {
        private Scanner _scanner;
        private Token _currentToken;

        public Parser(string pattern = null)
        {
            if (!string.IsNullOrEmpty(pattern))
                this._scanner = new Scanner(pattern);
        }

        private void Accept(TokenKind expectedKind)
        {
            if (this._currentToken.Kind == expectedKind)
            {
                this.AcceptIt();
                return;
            }

            throw new Exception("Parser error Unexpected TokenKind detected.");
        }

        private void AcceptIt()
        {
            if (this._scanner == null)
            {
                throw new Exception("No source text was provided");
            }
            this._currentToken = this._scanner.Scan();
        }

        private GlobNode ParseIdentifier()
        {
            if (this._currentToken.Kind == TokenKind.Identifier)
            {
                var identifier = new GlobNode(GlobNodeType.Identifier, this._currentToken.Spelling);
                this.AcceptIt();
                return identifier;
            }

            throw new Exception("Unable to parse Identifier");
        }

        private GlobNode ParseLiteralSet()
        {
            var items = new List<GlobNode>();
            this.Accept(TokenKind.LiteralSetStart);
            items.Add(this.ParseIdentifier());

            while (this._currentToken.Kind == TokenKind.LiteralSetSeperator)
            {
                this.AcceptIt();
                items.Add(this.ParseIdentifier());
            }
            this.Accept(TokenKind.LiteralSetEnd);
            return new GlobNode(GlobNodeType.LiteralSet, items);
        }

        private GlobNode ParseCharacterSet()
        {
            this.Accept(TokenKind.CharacterSetStart);
            var characterSet = this.ParseIdentifier();
            this.Accept(TokenKind.CharacterSetEnd);
            return new GlobNode(GlobNodeType.CharacterSet, characterSet);
        }

        private GlobNode ParseWildcard()
        {
            this.Accept(TokenKind.Wildcard);
            return new GlobNode(GlobNodeType.WildcardString);
        }

        private GlobNode ParseCharacterWildcard()
        {
            this.Accept(TokenKind.CharacterWildcard);
            return new GlobNode(GlobNodeType.CharacterWildcard);
        }

        private GlobNode ParseSubSegment()
        {
            switch (this._currentToken.Kind)
            {
                case TokenKind.Identifier:
                    return this.ParseIdentifier();
                case TokenKind.CharacterSetStart:
                    return this.ParseCharacterSet();
                case TokenKind.LiteralSetStart:
                    return this.ParseLiteralSet();
                case TokenKind.CharacterWildcard:
                    return this.ParseCharacterWildcard();
                case TokenKind.Wildcard:
                    return this.ParseWildcard();
                default:
                    throw new Exception("Unable to parse PathSubSegment");
            }
        }

        private GlobNode ParseSegment()
        {
            if (this._currentToken.Kind == TokenKind.DirectoryWildcard)
            {
                this.AcceptIt();
                return new GlobNode(GlobNodeType.DirectoryWildcard);
            }

            return ParsePathSegment();
        }

        private GlobNode ParsePathSegment()
        {
            var items = new List<GlobNode>();
            while (true)
            {
                switch (this._currentToken.Kind)
                {
                    case TokenKind.Identifier:
                    case TokenKind.CharacterSetStart:
                    case TokenKind.LiteralSetStart:
                    case TokenKind.CharacterWildcard:
                    case TokenKind.Wildcard:
                        items.Add(this.ParseSubSegment());
                        continue;
                    default:
                        break;
                }
                break;
            }

            return new GlobNode(GlobNodeType.PathSegment, items);
        }

        private GlobNode ParseRoot()
        {
            if (this._currentToken.Kind == TokenKind.PathSeperator)
                return new GlobNode(GlobNodeType.Root); //dont eat it so we can leave it for the segments


            if (this._currentToken.Kind == TokenKind.Identifier &&
               this._currentToken.Spelling.Length == 1 &&
               this._scanner.Peek().Kind == TokenKind.WindowsRoot)
            {
                var ident = this.ParseIdentifier();
                this.Accept(TokenKind.WindowsRoot);
                return new GlobNode(GlobNodeType.Root, ident);
            }

            return new GlobNode(GlobNodeType.Root, Directory.GetCurrentDirectory());
        }

        private GlobNode ParseTree()
        {
            var items = new List<GlobNode>();

            items.Add(this.ParseRoot());

            while (this._currentToken.Kind == TokenKind.PathSeperator)
            {
                this.AcceptIt();
                items.Add(this.ParseSegment());
            }

            return new GlobNode(GlobNodeType.Tree, items);
        }

        public GlobNode Parse(string text = null)
        {
            if (text != null)
                this._scanner = new Scanner(text);

            this.AcceptIt();
            var path = this.ParseTree();
            if (this._currentToken.Kind != TokenKind.EOT)
            {
                throw new Exception("Expected EOT");
            }

            return path;
        }
    }

}
