namespace Glob.AST
{
    internal class Root : Segment
    {
        public string Value { get; private set; }

        public Root(string value)
        {
            this.Value = value; //CWD
        }

        public Root(Identifier ident)
        {
            this.Value = ident.Value + ":"; //Windows
        }

        public Root()
        {
            this.Value = "/"; //linux
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
            if (this.Value == "/")
                return string.Empty;

            return this.Value;
        }
    }
}