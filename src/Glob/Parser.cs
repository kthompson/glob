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

        private Identifier ParseIdentifier()
        {
            if (this._currentToken.Kind == TokenKind.Identifier)
            {
                var identifier = new Identifier(this._currentToken.Spelling);
                this.AcceptIt();
                return identifier;
            }

            throw new Exception("Unable to parse Identifier");
        }

        private LiteralSet ParseLiteralSet()
        {
            var items = new List<Identifier>();
            this.Accept(TokenKind.LiteralSetStart);
            items.Add(this.ParseIdentifier());

            while (this._currentToken.Kind == TokenKind.LiteralSetSeperator)
            {
                this.AcceptIt();
                items.Add(this.ParseIdentifier());
            }
            this.Accept(TokenKind.LiteralSetEnd);
            return new LiteralSet(items);
        }

        private CharacterSet ParseCharacterSet()
        {
            this.Accept(TokenKind.CharacterSetStart);
            var inverted = false;
            if (this._currentToken.Kind == TokenKind.CharacterSetInvert)
            {
                this.AcceptIt();
                inverted = true;
            }
            var characterSet = this.ParseIdentifier();
            this.Accept(TokenKind.CharacterSetEnd);
            return new CharacterSet(characterSet, inverted);
        }

        private StringWildcard ParseWildcard()
        {
            this.Accept(TokenKind.Wildcard);
            return StringWildcard.Default;
        }

        private CharacterWildcard ParseCharacterWildcard()
        {
            this.Accept(TokenKind.CharacterWildcard);
            return CharacterWildcard.Default;
        }

        // SubSegment := Identifier | CharacterSet | LiteralSet | CharacterWildcard | Wildcard
        private SubSegment ParseSubSegment()
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
                    throw new Exception(
                        "Unable to parse SubSegment. " +
                        "   Expected one of Identifier | CharacterSet | LiteralSet | CharacterWildcard | Wildcard. " +
                        $"Found: {this._currentToken.Kind}"
                    );
            }
        }

        // Segment := DirectoryWildcard | DirectorySegment
        private Segment ParseSegment()
        {
            if (this._currentToken.Kind == TokenKind.DirectoryWildcard)
            {
                this.AcceptIt();
                return DirectoryWildcard.Default;
            }

            return ParseDirectorySegment();
        }

        // DirectorySegment := SubSegment SubSegment*
        private Segment ParseDirectorySegment()
        {
            var items = new List<SubSegment>
            {
                this.ParseSubSegment()
            };

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

            return new DirectorySegment(items);
        }

        private Root ParseRoot()
        {
            if (this._currentToken.Kind == TokenKind.PathSeparator)
                return new Root(); //dont eat it so we can leave it for the segments

            if (this._currentToken.Kind == TokenKind.WindowsRoot)
            {
                var root = new Root(this._currentToken.Spelling);
                this.Accept(TokenKind.WindowsRoot);
                return root;
            }

            return new Root();
        }

        // Tree := ( Root | Segment ) ( '/' Segment )*
        protected internal Tree ParseTree()
        {
            var items = new List<Segment>();

            if (this._currentToken.Kind == TokenKind.PathSeparator || this._currentToken.Kind == TokenKind.WindowsRoot)
            {
                items.Add(this.ParseRoot());
            }
            else
            {
                items.Add(this.ParseSegment());
            }

            while (this._currentToken.Kind == TokenKind.PathSeparator)
            {
                this.AcceptIt();
                items.Add(this.ParseSegment());
            }

            if (_currentToken.Kind != TokenKind.EOT)
                items.Add(this.ParseSegment());

            return new Tree(items);
        }

        public GlobNode Parse()
        {
            if (this._scanner == null)
                throw new InvalidOperationException("Scanner was not initialized. Ensure you are passing a pattern to Parse.");

            Tree path;

            switch (this._currentToken.Kind)
            {
                case TokenKind.WindowsRoot:
                case TokenKind.PathSeparator:
                case TokenKind.Identifier:
                case TokenKind.CharacterSetStart:
                case TokenKind.LiteralSetStart:
                case TokenKind.CharacterWildcard:
                case TokenKind.Wildcard:
                case TokenKind.DirectoryWildcard:
                    path = this.ParseTree();
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
