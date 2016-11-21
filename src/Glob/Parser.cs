using System;
using System.Collections.Generic;
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
            {
                InitializeScanner(pattern);
            }
        }

        private void InitializeScanner(string pattern)
        {
            this._scanner = new Scanner(pattern);
            this._currentToken = _scanner.Scan();
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
            
            if (this._currentToken.Kind == TokenKind.WindowsRoot)
            {
                var root = new GlobNode(GlobNodeType.Root, this._currentToken.Spelling);
                this.Accept(TokenKind.WindowsRoot);
                return root;
            }

            return new GlobNode(GlobNodeType.Root, "");
        }

        // Tree := ( Root | Segment ) ( '/' Segment )*
        private GlobNode ParseTree()
        {
            var items = new List<GlobNode>();

            if (this._currentToken.Kind == TokenKind.PathSeperator || this._currentToken.Kind == TokenKind.WindowsRoot)
            {
                items.Add(this.ParseRoot());
            }
            else
            {
                items.Add(this.ParseSegment());
            }

            while (this._currentToken.Kind == TokenKind.PathSeperator)
            {
                this.AcceptIt();
                items.Add(this.ParseSegment());
            }

            if (_currentToken.Kind != TokenKind.EOT)
                items.Add(this.ParseSegment());
            
            return new GlobNode(GlobNodeType.Tree, items);
        }

        public GlobNode Parse()
        {
            if(this._scanner == null)
                throw new InvalidOperationException("Scanner was not initialized. Ensure you are passing a pattern to Parse.");

            GlobNode path;

            switch (this._currentToken.Kind)
            {
                case TokenKind.WindowsRoot:
                case TokenKind.PathSeperator:
                case TokenKind.Identifier:
                case TokenKind.CharacterSetStart:
                case TokenKind.LiteralSetStart:
                case TokenKind.CharacterWildcard:
                case TokenKind.Wildcard:
                    path =  this.ParseTree();
                    break;
                default:
                    throw new InvalidOperationException("Expected Tree, found: " + _currentToken.Kind);
            }

            this.Accept(TokenKind.EOT);
            return path;
        }

        public GlobNode Parse(string text)
        {
            if (text != null)
                InitializeScanner(text);

            return this.Parse();
        }
    }
}
