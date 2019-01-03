using System;

namespace GlobExpressions
{
    [Flags]
    public enum GlobOptions
    {
        None = 0,
        Compiled = 1 << 1,
        CaseInsensitive = 1 << 2,
        MatchFullPath = 1 << 3,
    }
}
