using System;
using System.Linq;
using System.Text;

namespace GlobExpressions.AST
{
    internal sealed class CharacterSet : SubSegment
    {
        public bool Inverted { get; }
        public Identifier Characters { get; }
        public string ExpandedCharacters { get; }

        public CharacterSet(Identifier characters, bool inverted)
            : base(GlobNodeType.CharacterSet)
        {
            Characters = characters;
            Inverted = inverted;
            this.ExpandedCharacters = CalculateExpandedForm(characters.Value);
        }

        public bool Matches(char c, bool caseSensitive) => Contains(c, caseSensitive) != this.Inverted;

        private bool Contains(char c, bool caseSensitive) => ExpandedCharacters.IndexOf(c.ToString(), caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;

        private string CalculateExpandedForm(string chars)
        {
            if (!chars.Contains("-"))
                return chars;

            var sb = new StringBuilder();
            var i = 0;
            var len = chars.Length;
            while (true)
            {
                if (i >= len)
                    break;

                if (chars[i] == '-')
                {
                    if (i == len - 1)
                    {
                        sb.Append('-');
                    }
                    else
                    {
                        for (var c = chars[i - 1] + 1; c <= chars[i + 1]; c++)
                        {
                            sb.Append((char)c);
                        }
                        i++; // skip trailing range
                    }
                }
                else
                {
                    sb.Append(chars[i]);
                }
                i++;
            }

            return sb.ToString();
        }
    }
}
