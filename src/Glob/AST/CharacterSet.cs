using System;
using System.Linq;
using System.Text;

namespace GlobExpressions.AST;

internal sealed class CharacterSet : SubSegment
{
    public bool Inverted { get; }
    public string Characters { get; }
    public string ExpandedCharacters { get; }

    public CharacterSet(string characters, bool inverted)
        : base(GlobNodeType.CharacterSet)
    {
            Characters = characters;
            Inverted = inverted;
            this.ExpandedCharacters = CalculateExpandedForm(characters);
        }

    public bool Matches(char c, bool caseSensitive) => Contains(c, caseSensitive) != this.Inverted;

    private bool Contains(char c, bool caseSensitive) => ExpandedCharacters.IndexOf(c.ToString(), caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;

    private string CalculateExpandedForm(string chars)
    {
        var sb = new StringBuilder();
        var i = 0;
        var len = chars.Length;

        // if first character is special, add it
        if (chars.StartsWith("-") || chars.StartsWith("[") || chars.StartsWith("]"))
        {
            sb.Append(chars[0]);
            i++;
        }

        while (true)
        {
            if (i >= len)
                break;

            if (chars[i] == '-')
            {
                if (i == len - 1)
                {
                    // - is last character so just add it
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
            else if (chars[i] == '/')
            {
                i++; // skip
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