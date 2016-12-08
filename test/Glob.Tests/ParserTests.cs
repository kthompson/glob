using Xunit;

namespace Glob.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Issue3()
        {
            var parser = new Parser();
            var glob = parser.Parse("root/b.txt");

        }

        [Fact]
        public void CanParseSimpleFilename()
        {
            var parser = new Parser();
            var glob = parser.Parse("*.txt");
            Assert.Equal(GlobNodeType.Tree, glob.Type);
            var tree = Assert.IsType<Tree>(glob);
            Assert.Collection(tree.Segments, segment =>
            {
                Assert.Equal(GlobNodeType.PathSegment, segment.Type);

                Assert.Collection(segment.SubSegments, node =>
                {
                    Assert.Equal(GlobNodeType.StringWildcard, node.Type);
                    Assert.IsType<StringWildcard>(node);
                }, node =>
                {
                    Assert.Equal(GlobNodeType.Identifier, node.Type);
                    var ident = Assert.IsType<Identifier>(node);
                    Assert.Equal(".txt", ident.Value);
                });
            });
        }
    }
}