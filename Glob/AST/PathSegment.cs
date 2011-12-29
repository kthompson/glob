using System;
using System.Collections.Generic;
using System.Linq;

namespace Glob.AST
{
    public abstract class Segment : Node
    {
    }

    class PathSegment : Segment
    {
        public List<SubSegment> Items { get; private set; }

        public PathSegment(IEnumerable<SubSegment> items)
        {
            this.Items = new List<SubSegment>(items);
        }

        public override string ToGlobString()
        {
            return string.Join("", this.Items.Select(node => node.ToGlobString()));
        }

        public override bool IsWildcard
        {
            get { return this.Items.Any(node => node.IsWildcard); }
        }

        public override string ToRegexString()
        {
            return string.Join("", this.Items.Select(node => node.ToRegexString()));
        }
    }

    class LiteralSet : SubSegment
    {
        public List<Identifier> Items { get; set; }

        public LiteralSet(IEnumerable<Identifier> items)
        {
            this.Items = new List<Identifier>(items);
        }

        public override string ToGlobString()
        {
            return '{' + string.Join(",", this.Items.Select(item => item.ToGlobString())) + '}';
        }

        public override bool IsWildcard
        {
            get { return true; }
        }

        public override string ToRegexString()
        {
            return '(' + string.Join("|", this.Items.Select(item => item.ToRegexString())) + ')';
        }
    }
}