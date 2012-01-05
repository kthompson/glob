using System.Collections.Generic;

namespace Glob
{
    class GlobNode 
    {

        public GlobNode(GlobNodeType type, IEnumerable<GlobNode> children)
        {
            this.Type = type;
            this.Text = null;
            this.Children = new List<GlobNode>(children);
        }
        
        public GlobNode(GlobNodeType type)
        {
            this.Type = type;
            this.Text = null;
            this.Children = new List<GlobNode>();
        }


        public GlobNode(GlobNodeType type, GlobNode child)
        {
            this.Type = type;
            this.Text = null;
            this.Children = new List<GlobNode> {child};
        }

        public GlobNode(GlobNodeType type, string text)
        {
            this.Type = type;
            this.Text = text;
            this.Children = new List<GlobNode>();
        }

        public string Text { get; private set; }

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
        WildcardString, //string
        CharacterWildcard, //string
        DirectoryWildcard,
    }
}