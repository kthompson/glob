namespace GlobExpressions.AST
{
    internal abstract class GlobNode
    {
        protected GlobNode(GlobNodeType type)
        {
            this.Type = type;
        }

        public GlobNodeType Type { get; }
    }
}
