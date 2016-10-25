using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glob
{
    class Scanner
    {
        private readonly string _source;

        private int _sourceIndex;
        private char? _currentCharacter;
        private readonly StringBuilder _currentSpelling;
        private TokenKind _currentKind;

        public Scanner(string source)
        {
            this._source = source;
            this._sourceIndex = 0;
            this._currentSpelling = new StringBuilder();
            SetCurrentCharacter();
        }

        public void Take(char character)
        {
            if(this._currentCharacter == character)
            {
                this.TakeIt();
                return;
            }

            throw new InvalidDataException();
        }

        public void TakeIt()
        {
            this._currentSpelling.Append(this._currentCharacter);
            this._sourceIndex++;

            SetCurrentCharacter();
        }

        private void SetCurrentCharacter()
        {
            if (this._sourceIndex >= this._source.Length)
                this._currentCharacter = null;
            else
                this._currentCharacter = this._source[this._sourceIndex];
        }

        private char? PeekChar()
        {
            var sourceIndex = this._sourceIndex + 1;
            if (sourceIndex >= this._source.Length)
                return null;
            else
                return this._source[sourceIndex];
        }

        public Token Peek()
        {
            var index = this._sourceIndex;
            var token = this.Scan();
            this._sourceIndex = index;
            SetCurrentCharacter();
            return token;
        }

        public Token Scan()
        {
            this._currentSpelling.Clear();
            this._currentKind = this.ScanToken();

            return new Token(this._currentKind, this._currentSpelling.ToString());
        }

        private TokenKind ScanToken()
        {
            if(this._currentCharacter == null)
                return TokenKind.EOT;

            if (char.IsLetter(this._currentCharacter.Value) && this.PeekChar() == ':')
            {
                TakeIt(); // letter
                TakeIt(); // :
                return TokenKind.WindowsRoot;
            }

            if (IsAlphaNumeric(this._currentCharacter))
            {
                while (IsAlphaNumeric(this._currentCharacter))
                {
                    this.TakeIt();
                }

                return TokenKind.Identifier;
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
                    return TokenKind.PathSeperator;

                default:
                    throw new Exception("Unable to scan for next token. Stuck on '" + this._currentCharacter + "'");
            }
        }

        private static bool IsAlphaNumeric(char? c)
        {
            return c != null && (char.IsLetterOrDigit(c.Value) || c == '.');
        }
    }
}
