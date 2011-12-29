namespace Glob.AST
{
    public abstract class Node
    {
        public abstract string ToGlobString();

        public abstract bool IsWildcard { get;  }

        public abstract string ToRegexString();
    }
}