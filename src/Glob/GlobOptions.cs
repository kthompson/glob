using System;

namespace Glob
{
    [Flags]
    public enum GlobOptions
    {
        None = 0,
        Compiled = 1, 
        IgnoreCase = 2
    }
}