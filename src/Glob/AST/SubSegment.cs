namespace GlobExpressions.AST;

internal abstract class SubSegment : GlobNode
{
    protected SubSegment(GlobNodeType type)
        : base(type)
    {
        }
}