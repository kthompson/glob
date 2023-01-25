using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobExpressions;

enum SyntaxKind
{
    EndOfInputToken,

    CloseParenToken,
    OpenParenToken,
    OpenBraceToken,      // {
    CloseBraceToken,     // }

    CharacterSet,        // [...]

    QuestionToken,       // ?
    StarToken,           // *
    StarStarToken,       // **
    SlashToken,
    CommaToken,

    RootToken,
    LiteralToken,
}

record Token(SyntaxKind Kind, TextSpan Span, object Value);

class Lexer
{
    private readonly string _pattern;
    private int _position;
    private readonly Dictionary<char, Func<Token>> _lexFunctions = new();
    private readonly HashSet<char> _nonIdentChars = new();
    private readonly StringBuilder _currentIdentifier = new();

    // state variable representing start position or -1 for none
    private int _literalSetPos = -1;

    public Lexer(string pattern)
    {
        _pattern = pattern;
        InitializeLexer();
    }

    private void InitializeLexer()
    {
        _lexFunctions['{'] = ReturnOpenBraceToken;
        _lexFunctions['}'] = ReturnCloseBraceToken;
        _lexFunctions['['] = ReturnCharacterSetToken;
        _lexFunctions[']'] = ReturnCloseBracketToken;
        _lexFunctions['?'] = ReturnQuestionToken;
        _lexFunctions['*'] = ReturnStarToken;
        _lexFunctions['/'] = ReturnSlashToken;
        _lexFunctions[','] = ReturnCommaToken;

        foreach (char key in _lexFunctions.Keys)
            _nonIdentChars.Add(key);
    }

    private void CheckExtendedGlob(char current)
    {
        if (current != '(') return;

        // stub support for extended globs if we ever want to support it
        switch (Lookahead)
        {
            case '?':
            case '*':
            case '+':
            case '@':
            case '!':
                throw new GlobPatternException("Extended glob patterns are not currently supported");

            default:
                return;
        }
    }

    private Token ReturnOpenBraceToken()
    {
        if (InLiteralSet)
            throw new GlobPatternException($"Invalid nested literal set at offset {_position}");

        _literalSetPos = _position;
        return ReturnKindOneChar(SyntaxKind.OpenBraceToken);
    }

    private Token ReturnCloseBraceToken()
    {
        if (!InLiteralSet)
            throw new GlobPatternException($"Invalid literal set terminator at offset {_position}");

        _literalSetPos = -1;
        return ReturnKindOneChar(SyntaxKind.CloseBraceToken);
    }

    private Token ReturnCloseBracketToken()
    {
        throw new GlobPatternException($"Invalid character set terminator at offset {_position}");
    }

    private Token ReturnCharacterSetToken()
    {
        _position++; // accept [

        var start = _position;
        var inverted = false;


        if (Current == null)
        {
            throw new GlobPatternException($"Unterminated character set at offset {start}");
        }

        if(Current.Value == '!')
        {
            _position++;
            start++; // dont count the `!` in the character set
            inverted = true;
        }

        if (Current == null)
        {
            throw new GlobPatternException($"Unterminated character set at offset {start}");
        }

        // first token is special and we allow more things like ] or [ at the beginning
        if (Current.Value == ']')
        {
            _position++;
        }

        while (true)
        {
            if (Current == null)
            {
                throw new GlobPatternException($"Unterminated character set at offset {start}");
            }

            if (Current.Value != ']')
            {
                _position++;
                continue;
            }

            break;
        }

        var token = new Token(SyntaxKind.CharacterSet, TextSpan.FromBounds(start, _position), inverted);

        _position++; // accept `]`

        return token;
    }

    private bool InLiteralSet => _literalSetPos >= 0;

    private Token ReturnQuestionToken() => ReturnKindOneChar(SyntaxKind.QuestionToken);

    private Token ReturnStarToken() => Lookahead == '*'
        ? ReturnKindTwoChar(SyntaxKind.StarStarToken)
        : ReturnKindOneChar(SyntaxKind.StarToken);

    private Token ReturnCommaToken() => ReturnKindOneChar(SyntaxKind.CommaToken);
    private Token ReturnSlashToken() => ReturnKindOneChar(SyntaxKind.SlashToken);

    private Token ReturnEndOfInput() =>
        new Token(SyntaxKind.EndOfInputToken, TextSpan.FromBounds(_position, _position), string.Empty);
    private Token ReturnKindOneChar(SyntaxKind kind)
    {
        var start = _position;
        _position++;
        return new Token(kind, TextSpan.FromBounds(start, _position), string.Empty);
    }

    private Token ReturnKindTwoChar(SyntaxKind kind)
    {
        var start = _position;
        _position += 2;
        return new Token(kind, TextSpan.FromBounds(start, _position), string.Empty);
    }

    private char? Current => Peek(_position);
    private char? Lookahead => Peek(_position + 1);
    private char? Peek(int position) => position >= _pattern.Length ? null : _pattern[position];

    private bool IsIdentCharacter(char current, bool inCharacterSet)
    {
        // if we are in a literal set we parse commas as their own token, otherwise
        // they are considered an identifier character
        if (current == ',')
            return !InLiteralSet;

        // character wildcards are treated as ident characters in character sets
        if (current == '?')
            return inCharacterSet;

        return !_nonIdentChars.Contains(current);
    }

    private Token ParseIdentToken(bool inCharacterSet)
    {
        var start = _position;
        _currentIdentifier.Clear();

        while (true)
        {
            if (Current == null)
                break;

            if (Current == '\\')
            {
                var escapeSequence = ParseEscapeSequence(inCharacterSet);
                _currentIdentifier.Append(escapeSequence);
            }
            else if (IsIdentCharacter(Current.Value, inCharacterSet))
            {
                _currentIdentifier.Append(Current.Value);
                _position++;
            }
            else
            {
                break;
            }
        }

        return new Token(SyntaxKind.LiteralToken, TextSpan.FromBounds(start, _position), _currentIdentifier.ToString());
    }

    private string ParseEscapeSequence(bool inCharacterSet)
    {
        _position++; // accept \
        switch (Current)
        {
            case '*':
            case '?':
            case '{':
            case '}':
            case '[':
            case ']':
            case '(':
            case ')':
            case ' ':
            case ',' when inCharacterSet:
                var result = Current.Value.ToString();
                _position++;
                return result;

            default:
                throw new GlobPatternException(
                    $"Expected escape sequence at index pattern `{_position}` but found `\\{Current}`");
        }
    }

    public Token ParseToken()
    {
        if (Current == null) return ReturnEndOfInput();

        if (_position == 0)
        {
            var token = TryParseRootToken();
            if (token != null)
                return token;
        }

        CheckExtendedGlob(Current.Value);

        if (IsIdentCharacter(Current.Value, false))
            return ParseIdentToken(false);

        if (_lexFunctions.TryGetValue(Current.Value, out var function))
        {
            return function();
        }

        throw new GlobPatternException($"Unexpected character {Current} at index {_position}");
    }

    private Token? TryParseRootToken()
    {
        if (Current == null)
        {
            return null;
        }

        // osx/linux root
        if (Current == '/')
        {
            _position += 1;
            return new Token(SyntaxKind.RootToken, TextSpan.FromBounds(0, 0), string.Empty);
        }

        // windows root
        if (char.IsLetter(Current.Value) && Lookahead == ':')
        {
            _position += 2;
            return new Token(SyntaxKind.RootToken, TextSpan.FromBounds(0, 2), string.Empty);
        }

        return null;
    }
}
