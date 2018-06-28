using Xunit;

namespace GlobExpressions.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Issue3()
        {
            Parse("root/b.txt");
        }

        private static GlobNode Parse(string pattern)
        {
            var parser = new Parser();
            var glob = parser.Parse(pattern);
            return glob;
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
                Assert.Equal(GlobNodeType.DirectorySegment, segment.Type);
                var directory = Assert.IsType<DirectorySegment>(segment);

                Assert.Collection(directory.SubSegments, node =>
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

        [Fact]
        public void CanParseStarStarBin()
        {
            var glob = Parse("**/bin");
            var tree = Assert.IsType<Tree>(glob);
            Assert.Collection(tree.Segments,
                segment => Assert.Same(DirectoryWildcard.Default, segment),
                segment =>
                {
                    var dir = Assert.IsType<DirectorySegment>(segment);
                    Assert.Collection(dir.SubSegments,
                        subSegment =>
                        {
                            var identifier = Assert.IsType<Identifier>(subSegment);
                            Assert.Equal("bin", identifier.Value);
                        });
                });
        }

        [Fact]
        public void CanParseThis1()
        {
            var x = new CharacterSet("a-fG-L", false);
            AssertIdentifier(x.Characters, "a-fG-L");
            Assert.Equal("abcdefGHIJKL", x.ExpandedCharacters);
        }

        [Fact]
        public void CanParseThis()
        {
            var glob = Parse("*fil[e-z].txt");
            var tree = Assert.IsType<Tree>(glob);
            Assert.Collection(tree.Segments,
                segment =>
                {
                    var dir = Assert.IsType<DirectorySegment>(segment);
                    Assert.Collection(dir.SubSegments,
                        subSegment => Assert.Same(StringWildcard.Default, subSegment),
                        subSegment => AssertIdentifier(subSegment, "fil"),
                        subSegment =>
                        {
                            var set = Assert.IsType<CharacterSet>(subSegment);
                            Assert.False(set.Inverted);
                            AssertIdentifier(set.Characters, "e-z");
                            Assert.Equal("efghijklmnopqrstuvwxyz", set.ExpandedCharacters);
                        },
                        subSegment => AssertIdentifier(subSegment, ".txt"));
                });
        }

        private static void AssertIdentifier(SubSegment subSegment, string expected)
        {
            var identifier = Assert.IsType<Identifier>(subSegment);

            Assert.Equal(expected, identifier.Value);
        }
    }
}
