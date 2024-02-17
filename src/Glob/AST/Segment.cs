namespace GlobExpressions.AST;

internal abstract class Segment : GlobNode
{
    protected Segment(GlobNodeType type)
        : base(type)
    {
        }
}