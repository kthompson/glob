namespace GlobExpressions.AST
{
    internal sealed class StringWildcard : SubSegment
    {
        public static readonly StringWildcard Default = new StringWildcard();

        private StringWildcard()
            : base(GlobNodeType.StringWildcard)
        {
        }
    }
}
