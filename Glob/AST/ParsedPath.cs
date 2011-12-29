using System.Collections.Generic;
using System.Linq;

namespace Glob.AST
{
    public class ParsedPath : Node
    {
        public List<Segment> Items { get; set; }

        public ParsedPath(IEnumerable<Segment> items)
        {
            this.Items = new List<Segment>(items);
        }

        public override string ToGlobString()
        {
            return string.Join("/", this.Items.Select(item => item.ToGlobString()));
        }

        public override bool IsWildcard
        {
            get { return this.Items.Any(node => node.IsWildcard); }
        }

        public override string ToRegexString()
        {
            return string.Join("/", this.Items.Select(item => item.ToRegexString()));
        }
    }
}