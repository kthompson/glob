using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glob
{
    class Scanner
    {
        private readonly string _source;

        private int _sourceIndex;
        private int _currentCharacter;
        private int _startIndex;
        private TokenKind _currentKind;

        public Scanner(string source)
        {
            this._source = source;
            this._sourceIndex = 0;
            this._startIndex = 0;
            SetCurrentCharacter();
        }

        public void TakeIt()
        {
            this._sourceIndex++;

            SetCurrentCharacter();
        }

        private void SetCurrentCharacter()
        {
            if (this._sourceIndex >= this._source.Length)
                this._currentCharacter = -1;
            else
                this._currentCharacter = this._source[this._sourceIndex];
        }

        private int PeekChar()
        {
            var sourceIndex = this._sourceIndex + 1;
            if (sourceIndex >= this._source.Length)
                return -1;

            return this._source[sourceIndex];
        }

        public Token Scan()
        {
            this._currentKind = this.ScanToken();

            var spelling = _source.Substring(_startIndex, _sourceIndex - _startIndex);

            _startIndex = _sourceIndex;

            return new Token(_currentKind, spelling);
        }

        private TokenKind ScanToken()
        {
            if(this._currentCharacter == -1)
                return TokenKind.EOT;

            var current = (char) _currentCharacter;

            if (char.IsLetter(current))
            {
                if (_sourceIndex == 0 && this.PeekChar() == ':')
                {
                    TakeIt(); // letter
                    TakeIt(); // :
                    return TokenKind.WindowsRoot;
                }

                return TakeIdentifier();
            }

            if (IsNumeric(current) || current == '_')
            {
                return TakeIdentifier();
            }

            switch (this._currentCharacter)
            {
                case '*':
                    this.TakeIt();
                    if (this._currentCharacter == '*')
                    {
                        this.TakeIt();
                        return TokenKind.DirectoryWildcard;
                    }

                    return TokenKind.Wildcard;
                case '?':
                    this.TakeIt();
                    return TokenKind.CharacterWildcard;

                case '!':
                    this.TakeIt();
                    return TokenKind.CharacterSetInvert;

                case '[':
                    this.TakeIt();
                    return TokenKind.CharacterSetStart;

                case ']':
                    this.TakeIt();
                    return TokenKind.CharacterSetEnd;

                case '{':
                    this.TakeIt();
                    return TokenKind.LiteralSetStart;

                case ',':
                    this.TakeIt();
                    return TokenKind.LiteralSetSeperator;

                case '}':
                    this.TakeIt();
                    return TokenKind.LiteralSetEnd;

                case '/':
                case '\\':
                    this.TakeIt();
                    return TokenKind.PathSeparator;

                default:
                    throw new Exception("Unable to scan for next token. Stuck on '" + (char)this._currentCharacter + "'");
            }
        }

        private TokenKind TakeIdentifier()
        {
            
            while (IsIdentifierCharacter((char)this._currentCharacter))
            {
                this.TakeIt();
            }

            return TokenKind.Identifier;
        }

        private static bool IsIdentifierCharacter(char c) => char.IsLetter(c) || IsNumeric(c) || c == '_';

        private static bool IsNumeric(char c) => char.IsDigit(c) || c == '.' || c == '-';
    }
}
