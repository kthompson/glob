namespace GlobExpressions.AST
{
    internal sealed class Root : Segment
    {
        public string Text { get; }

        public Root(string text = "")
            : base(GlobNodeType.Root)
        {
            Text = text;
        }

        public override string ToString() => Text;
    }
}
