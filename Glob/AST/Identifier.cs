namespace Glob.AST
{
    internal class Identifier : SubSegment
    {
        public string Value { get; private set; }

        public Identifier(string value)
        {
            this.Value = value;
        }

        public override string ToGlobString()
        {
            return this.Value;
        }

        public override bool IsWildcard
        {
            get { return false; }
        }

        public override string ToRegexString()
        {
            return this.Value.Replace(".", @"\.");
        }
    }
}