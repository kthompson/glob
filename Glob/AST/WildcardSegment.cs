namespace Glob.AST
{
    internal class WildcardSegment : Segment
    {
        public override bool IsWildcard
        {
            get
            {
                return true;
            }
        }

        public override string ToGlobString()
        {
            return "**";
        }

        public override string ToRegexString()
        {
            return ".*";
        }
    }
}