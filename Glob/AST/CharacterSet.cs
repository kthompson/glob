using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glob.AST
{
    class CharacterSet :  SubSegment
    {
        public Identifier Identifier { get; set; }

        public CharacterSet(Identifier identifier)
        {
            this.Identifier = identifier;
        }

        public override bool IsWildcard
        {
            get
            {
                return true;
            }
        }

        public override string ToGlobString()
        {
            return "[" + this.Identifier.ToGlobString() + "]";
        }

        public override string ToRegexString()
        {
            return "[" + this.Identifier.ToRegexString() + "]";
        }
    }
}
