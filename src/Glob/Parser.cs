using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GlobExpressions.AST;

namespace GlobExpressions
{
    internal class Parser
    {
        private readonly string _pattern;
        private readonly ImmutableArray<Token> _tokens;
        private int _tokenPosition;

        public Parser(string pattern)
        {
            _pattern = pattern;
            var lexer = new Lexer(pattern);
            var tokens = ImmutableArray.CreateBuilder<Token>();
            while (true)
            {
                var token = lexer.ParseToken();
                tokens.Add(token);
                if (token.Kind == SyntaxKind.EndOfInputToken)
                    break;
            }

            _tokens = tokens.ToImmutable();
        }

        private Token CurrentToken => _tokenPosition > _tokens.Length - 1 ? _tokens[^1] : _tokens[_tokenPosition];
        private SyntaxKind CurrentKind => CurrentToken.Kind;

        private void NextToken() => _tokenPosition += 1;
        private Token Accept()
        {
            var token = CurrentToken;
            NextToken();
            return token;
        }

        private string FromPattern(int start, int length) => _pattern.Substring(start, length);

        private string FromPattern(TextSpan span) => FromPattern(span.Start, span.Length);

        private StringWildcard ParseStar()
        {
            Accept();
            return StringWildcard.Default;
        }

        private CharacterWildcard ParseCharacterWildcard()
        {
            Accept();
            return CharacterWildcard.Default;
        }

        private CharacterSet ParseCharacterSet()
        {
            var token = Accept();
            var inverted = (bool)token.Value;

            return new CharacterSet(_pattern.Substring(token.Span.Start, token.Span.Length), inverted);
        }

        private Token Accept(SyntaxKind kind)
        {
            var currentToken = CurrentToken;
            if (currentToken.Kind == kind)
                return Accept();

            return ThrowUnexpectedToken();
        }

        private Token ThrowUnexpectedToken()
        {
            throw new GlobPatternException($"Unexpected token {CurrentKind} at offset {CurrentToken.Span.Start}");
        }

        private LiteralSet ParseLiteralSet()
        {
            var items = new List<Identifier>();
            Accept(); // {
            var expected = SyntaxKind.LiteralToken;


            while (CurrentKind != SyntaxKind.CloseBraceToken && CurrentKind != SyntaxKind.EndOfInputToken)
            {
                if (expected == SyntaxKind.LiteralToken)
                {
                    switch (CurrentKind)
                    {
                        case SyntaxKind.LiteralToken:
                            items.Add(ParseIdentifier());
                            expected = SyntaxKind.CommaToken;
                            break;
                        case SyntaxKind.CommaToken:
                            items.Add(new Identifier(string.Empty)); // add empty, accept comma, expect literal
                            Accept();
                            expected = SyntaxKind.LiteralToken;
                            break;
                        default:
                            ThrowUnexpectedToken();
                            break;
                    }
                }
                else if (expected == SyntaxKind.CommaToken)
                {
                    switch (CurrentKind)
                    {
                        case SyntaxKind.CommaToken:
                            Accept();
                            expected = SyntaxKind.LiteralToken;
                            break;
                        default:
                            ThrowUnexpectedToken();
                            break;
                    }
                }
            }

            if (expected == SyntaxKind.LiteralToken)
            {
                items.Add(new Identifier(string.Empty)); // add empty
                if (items.Count == 1)
                {
                    throw new GlobPatternException("Expected literal set with at least one value");
                }
            }

            Accept(SyntaxKind.CloseBraceToken); // }
            if (items.Count == 0)
                throw new GlobPatternException("Empty literal set");

            return new LiteralSet(items);
        }

        private Identifier ParseIdentifier()
        {
            var identifier = Accept();
            return new Identifier((string)identifier.Value);
        }

        // SubSegment := Identifier | CharacterSet | LiteralSet | CharacterWildcard | Wildcard | DirectoryWildcard
        private SubSegment ParseSubSegment() =>
            CurrentKind switch
            {
                SyntaxKind.CharacterSet => this.ParseCharacterSet(),
                SyntaxKind.OpenBraceToken => this.ParseLiteralSet(),
                SyntaxKind.QuestionToken => this.ParseCharacterWildcard(),
                SyntaxKind.StarToken => this.ParseStar(),
                SyntaxKind.LiteralToken => this.ParseIdentifier(),
                _ => throw new GlobPatternException(
                    $"Unexpected token {CurrentKind} at offset {CurrentToken.Span.Start}")
            };

        // Segment := DirectorySegment | DirectoryWildcard
        private Segment ParseSegment()
        {
            var directorySegment = ParseDirectorySegment();
            return directorySegment == null ? DirectoryWildcard.Default : directorySegment;
        }

        /// <summary>
        /// DirectorySegment := SubSegment*
        /// </summary>
        /// <returns>a DirectorySegment or null if a DirectoryWildcard is detected</returns>
        private DirectorySegment? ParseDirectorySegment()
        {
            var subsegments = new List<SubSegment>();
            while (true)
            {
                switch (CurrentKind)
                {
                    case SyntaxKind.LiteralToken:
                    case SyntaxKind.OpenBraceToken:
                    case SyntaxKind.CharacterSet:
                    case SyntaxKind.StarToken:
                    case SyntaxKind.QuestionToken:
                        subsegments.Add(ParseSubSegment());
                        continue;

                    case SyntaxKind.StarStarToken when subsegments.Count > 0:
                        // `**` grouped with literals, demote `**` to `*`
                        subsegments.Add(ParseStar());
                        continue;

                    case SyntaxKind.StarStarToken:
                        Accept();
                        if (CurrentKind == SyntaxKind.SlashToken || CurrentKind == SyntaxKind.EndOfInputToken)
                        {
                            // return null signifies a DirectoryWildcard
                            return null;
                        }
                        // `**` preceding other literals, demote `**` to `*`
                        subsegments.Add(StringWildcard.Default);
                        continue;
                }

                break;
            }

            return new DirectorySegment(subsegments);

        }

        private Root ParseRoot()
        {
            var root = Accept();
            var text = FromPattern(root.Span);
            return new Root(text);
        }

        // Tree := Root? (Segment ('/' Segment)*)?
        protected internal Tree ParseTree()
        {
            var items = new List<Segment>();

            if (CurrentKind == SyntaxKind.RootToken)
            {
                items.Add(this.ParseRoot());
                if (CurrentKind == SyntaxKind.SlashToken)
                    Accept();
            }

            if (CurrentKind == SyntaxKind.EndOfInputToken)
            {
                return new Tree(items);
            }

            items.Add(this.ParseSegment());

            while (CurrentKind == SyntaxKind.SlashToken)
            {
                Accept();
                items.Add(this.ParseSegment());
            }

            if (CurrentKind != SyntaxKind.EndOfInputToken)
            {
                ThrowUnexpectedToken();
            }

            return new Tree(items);
        }

        public GlobNode Parse() => this.ParseTree();
    }
}
