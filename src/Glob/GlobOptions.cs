using System;

namespace GlobExpressions
{
    [Flags]
    public enum GlobOptions
    {
        None,
        Compiled,
        CaseInsensitive
    }
}
