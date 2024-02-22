﻿using GlobExpressions.AST;
using Xunit;

namespace GlobExpressions.Tests;

public class ParserTests
{
    [Fact]
    public void Issue3()
    {
        Parse("root/b.txt");
    }

    private static GlobNode Parse(string pattern)
    {
        var parser = new Parser(pattern);
        return parser.Parse();
    }

    [Fact]
    public void CanParsePatternWithSpace()
    {
        var glob = Parse(@"Generated\ Files");
        Assert.Equal(GlobNodeType.Tree, glob.Type);
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments, segment =>
        {
            Assert.Equal(GlobNodeType.DirectorySegment, segment.Type);
            var directory = Assert.IsType<DirectorySegment>(segment);

            Assert.Collection(directory.SubSegments, node =>
            {
                Assert.Equal(GlobNodeType.Identifier, node.Type);
                var ident = Assert.IsType<Identifier>(node);
                Assert.Equal("Generated Files", ident.Value);
            });
        });
    }

    [Fact]
    public void CanParseSimpleFilename()
    {
        var glob = Parse("*.txt");
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
    public void CanParseLinuxRoot()
    {
        var glob = Parse("/bin");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment => Assert.IsType<Root>(segment),
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "bin"));
            });
    }

    [Fact]
    public void CanParseWindowsRoot()
    {
        var glob = Parse("C:/bin");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment => Assert.Equal("C:", Assert.IsType<Root>(segment).Text),
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "bin"));
            });
    }


    [Fact]
    public void CanParseLiteralSetWithEmpty1()
    {
        var glob = Parse("{,c}");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment =>
                    {
                        var literalSet = Assert.IsType<LiteralSet>(subSegment);
                        Assert.Collection(literalSet.Literals,
                            literal => AssertIdentifier(literal, ""),
                            literal => AssertIdentifier(literal, "c"));
                    });
            });
    }

    [Fact]
    public void CanParseLiteralSetWithEmpty2()
    {
        var glob = Parse("{c,}");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment =>
                    {
                        var literalSet = Assert.IsType<LiteralSet>(subSegment);
                        Assert.Collection(literalSet.Literals,
                            literal => AssertIdentifier(literal, "c"),
                            literal => AssertIdentifier(literal, ""));
                    });
            });
    }

    [Fact]
    public void ThrowsForEmptyLiteralSet()
    {
        var error = Assert.Throws<GlobPatternException>(() => Parse("{}"));
        Assert.Equal("Expected literal set with at least one value", error.Message);
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
        Assert.Equal("a-fG-L", x.Characters);
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
                        Assert.Equal("e-z", set.Characters);
                        Assert.Equal("efghijklmnopqrstuvwxyz", set.ExpandedCharacters);
                    },
                    subSegment => AssertIdentifier(subSegment, ".txt"));
            });
    }

    [Fact]
    public void CanParseCharacterSetWithSpecials()
    {
        var glob = Parse(@"[\*?]");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment =>
                    {
                        var set = Assert.IsType<CharacterSet>(subSegment);
                        Assert.False(set.Inverted);
                        Assert.Equal(@"\*?", set.Characters);
                        Assert.Equal(@"\*?", set.ExpandedCharacters);
                    });
            });
    }

    [Fact]
    public void CanParseCharacterSetWithStart()
    {
        var glob = Parse("[[]");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment =>
                    {
                        var set = Assert.IsType<CharacterSet>(subSegment);
                        Assert.False(set.Inverted);
                        Assert.Equal("[", set.Characters);
                        Assert.Equal("[", set.ExpandedCharacters);
                    });
            });
    }

    [Fact]
    public void CanParseCharacterSetWithEnd()
    {
        var glob = Parse("[]]");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment =>
                    {
                        var set = Assert.IsType<CharacterSet>(subSegment);
                        Assert.False(set.Inverted);
                        Assert.Equal("]", set.Characters);
                        Assert.Equal("]", set.ExpandedCharacters);
                    });
            });
    }

    [Fact]
    public void CanParseCharacterSetWithSeparator()
    {
        var glob = Parse("[-]");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment =>
                    {
                        var set = Assert.IsType<CharacterSet>(subSegment);
                        Assert.False(set.Inverted);
                        Assert.Equal("-", set.Characters);
                        Assert.Equal("-", set.ExpandedCharacters);
                    });
            });
    }

    [Fact]
    public void CanParseEmptyString()
    {
        var node = Parse("");
        var tree = Assert.IsType<Tree>(node);
        Assert.Empty(tree.Segments);
    }

    [Fact]
    public void CanParseDoubleWildcard()
    {
        var glob = Parse("a**/*.cs");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "a"),
                    subSegment => Assert.Same(StringWildcard.Default, subSegment)
                );
            },
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => Assert.Same(StringWildcard.Default, subSegment),
                    subSegment => AssertIdentifier(subSegment, ".cs")
                );
            });
    }

    [Fact]
    public void CanParseDoubleWildcardPrefix()
    {
        var glob = Parse("**a/*.cs");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => Assert.Same(StringWildcard.Default, subSegment),
                    subSegment => AssertIdentifier(subSegment, "a")
                );
            },
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => Assert.Same(StringWildcard.Default, subSegment),
                    subSegment => AssertIdentifier(subSegment, ".cs")
                );
            }
        );
    }

    [Fact]
    public void CanParseEscapedBrackets()
    {
        var glob = Parse(@"\[a-d\]");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "[a-d]")
                );
            }
        );
    }

    [Fact]
    public void CanParseEscapedBraces()
    {
        var glob = Parse(@"\{ab,bc\}");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "{ab,bc}")
                );
            }
        );
    }

    [Fact]
    public void CanParseCharacterWildcard()
    {
        var glob = Parse(@"?");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                Assert.Collection(Assert.IsType<DirectorySegment>(segment).SubSegments,
                    subSegment => Assert.IsType<CharacterWildcard>(subSegment));
            }
        );
    }

    [Fact]
    public void CanParseStringWildcard()
    {
        var glob = Parse(@"*");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => Assert.IsType<StringWildcard>(subSegment));
            }
        );
    }

    [Fact]
    public void CanParseDirectoryWildcard()
    {
        var glob = Parse(@"**");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment => Assert.IsType<DirectoryWildcard>(segment));
    }

    [Fact]
    public void CanParseEscapedCharacterWildcard()
    {
        var glob = Parse(@"hat\?");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "hat?")
                );
            }
        );
    }

    [Fact]
    public void CanParseEscapedWildcard()
    {
        var glob = Parse(@"hat\*");
        var tree = Assert.IsType<Tree>(glob);
        Assert.Collection(tree.Segments,
            segment =>
            {
                var dir = Assert.IsType<DirectorySegment>(segment);
                Assert.Collection(dir.SubSegments,
                    subSegment => AssertIdentifier(subSegment, "hat*")
                );
            }
        );
    }

    [Theory]
    [InlineData("]")]
    [InlineData("[")]
    [InlineData("}")]
    [InlineData("{")]
    public void InvalidPatternThrows(string pattern)
    {
        Assert.Throws<GlobPatternException>(() => Parse(pattern));
    }

    private static void AssertIdentifier(SubSegment subSegment, string expected)
    {
        var identifier = Assert.IsType<Identifier>(subSegment);

        Assert.Equal(expected, identifier.Value);
    }
}