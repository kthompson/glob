namespace GlobExpressions.AST
{
    internal sealed class Identifier : SubSegment
    {
        public string Value { get; }

        public Identifier(string value)
            : base(GlobNodeType.Identifier)
        {
            Value = value;
        }

        public static implicit operator Identifier(string value)
        {
            return new Identifier(value);
        }
    }
}
