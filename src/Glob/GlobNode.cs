using System.Collections.Generic;
using System.Linq;

namespace Glob
{

    class StringWildcard : GlobNode
    {
        public StringWildcard()
            : base(GlobNodeType.StringWildcard)
        {

        }
    }

    class CharacterWildcard : GlobNode
    {
        public CharacterWildcard()
            : base(GlobNodeType.CharacterWildcard)
        {

        }
    }

    class DirectoryWildcard : GlobNode
    {
        public DirectoryWildcard()
            : base(GlobNodeType.DirectoryWildcard)
        {

        }
    }

    class Root : GlobNode
    {
        public string Text { get; }

        public Root(string text = "")
            : base(GlobNodeType.Root)
        {
            Text = text;
        }
    }

    class CharacterSet : GlobNode
    {
        public bool Inverted { get; }
        public Identifier Characters { get; }

        public CharacterSet(Identifier characters, bool inverted)
            : base(GlobNodeType.CharacterSet)
        {
            Characters = characters;
            Inverted = inverted;
        }
    }

    class Identifier : GlobNode
    {
        public string Value { get; }

        public Identifier(string value) 
            : base(GlobNodeType.Identifier)
        {
            Value = value;
        }
    }

    class LiteralSet : GlobNode
    {
        public IEnumerable<Identifier> Literals { get; }

        public LiteralSet(IEnumerable<Identifier> literals) 
            : base(GlobNodeType.Identifier)
        {
            Literals = literals.ToList();
        }
    }

    class GlobNode 
    {

        public GlobNode(GlobNodeType type, IEnumerable<GlobNode> children)
        {
            this.Type = type;
            this.Children = new List<GlobNode>(children);
        }

        protected GlobNode(GlobNodeType type)
        {
            this.Type = type;
            this.Children = new List<GlobNode>();
        }

        public GlobNodeType Type { get; private set; }

        public List<GlobNode> Children { get; private set; }
    }

    enum GlobNodeType
    {
        CharacterSet, //string, no children
        Tree, // children
        Identifier, //string
        LiteralSet, //children
        PathSegment, //children
        Root, //string 
        StringWildcard, //string
        CharacterWildcard, //string
        DirectoryWildcard,
    }
}