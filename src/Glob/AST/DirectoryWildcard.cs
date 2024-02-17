namespace GlobExpressions.AST;

internal sealed class DirectoryWildcard : Segment
{
    public static readonly DirectoryWildcard Default = new DirectoryWildcard();

    private DirectoryWildcard()
        : base(GlobNodeType.DirectoryWildcard)
    {
        }

    public override string ToString()
    {
            return "**";
        }
}