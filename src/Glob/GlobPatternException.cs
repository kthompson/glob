using System;

namespace GlobExpressions;

public class GlobPatternException : Exception
{
    internal GlobPatternException(string message)
        : base(message)
    {
        }
}