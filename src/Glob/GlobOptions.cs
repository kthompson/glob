using System;

namespace GlobExpressions
{
    [Flags]
    public enum GlobOptions
    {
        None,
        Compiled,
        MatchFullPath,
        CaseInsensitive
    }
}
