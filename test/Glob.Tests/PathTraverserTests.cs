using System;
using System.Collections.Generic;
using System.Text;
using GlobExpressions.AST;
using Xunit;

namespace GlobExpressions.Tests
{
    public class PathTraverserTests
    {
        [Fact]
        public void ShouldMatchStringWildcard()
        {
            // *
            var list = new DirectorySegment(new SubSegment[]
            {
                StringWildcard.Default,
            });

            Assert.True(list.MatchesSegment(""));
            Assert.True(list.MatchesSegment("a"));
            Assert.True(list.MatchesSegment("abc"));
        }

        [Fact]
        public void ShouldMatchIdentWildcard()
        {
            // ab*cd
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                StringWildcard.Default,
                new Identifier("cd"),
            });

            Assert.True(list.MatchesSegment("abcd"));
            Assert.True(list.MatchesSegment("abcdcd"));
            Assert.True(list.MatchesSegment("ab123456cd"));

            Assert.False(list.MatchesSegment("ab123456cd11"));
            Assert.False(list.MatchesSegment("abcd1"));
            Assert.False(list.MatchesSegment("abcdcd1"));
            Assert.False(list.MatchesSegment("ab123456cd1"));
        }

        [Fact]
        public void ShouldMatchLiteralSet()
        {
            // ab*(cd|ef)
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                StringWildcard.Default,
                new LiteralSet("cd", "ef"),
            });

            Assert.True(list.MatchesSegment("abcd"));
            Assert.True(list.MatchesSegment("abcdcd"));
            Assert.True(list.MatchesSegment("ab123456cd"));

            Assert.True(list.MatchesSegment("abef"));
            Assert.True(list.MatchesSegment("abcdef"));
            Assert.True(list.MatchesSegment("ab123456ef"));

            Assert.False(list.MatchesSegment("ab123456cd11"));
            Assert.False(list.MatchesSegment("abcd1"));
            Assert.False(list.MatchesSegment("abcdcd1"));
            Assert.False(list.MatchesSegment("ab123456cd1"));
        }

        [Fact]
        public void ShouldMatchCharacterWildcard()
        {
            // ab?
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                CharacterWildcard.Default
            });

            Assert.True(list.MatchesSegment("abc"));
            Assert.True(list.MatchesSegment("abd"));
            Assert.True(list.MatchesSegment("ab1"));

            Assert.False(list.MatchesSegment("eab"));
            Assert.False(list.MatchesSegment("abef"));
            Assert.False(list.MatchesSegment("ab123456cd11"));
            Assert.False(list.MatchesSegment("abcd1"));
            Assert.False(list.MatchesSegment("abcdcd1"));
            Assert.False(list.MatchesSegment("ab123456cd1"));
        }

        [Fact]
        public void ShouldMatchCharacterSet()
        {
            // ab?[abc]
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                CharacterWildcard.Default,
                new CharacterSet("abc", false)
            });

            Assert.True(list.MatchesSegment("abca"));
            Assert.True(list.MatchesSegment("abda"));
            Assert.True(list.MatchesSegment("ab1a"));

            Assert.True(list.MatchesSegment("abcb"));
            Assert.True(list.MatchesSegment("abdb"));
            Assert.True(list.MatchesSegment("ab1b"));

            Assert.True(list.MatchesSegment("abcc"));
            Assert.True(list.MatchesSegment("abdc"));
            Assert.True(list.MatchesSegment("ab1c"));
        }

        [Fact]
        public void ShouldMatchCharacterSetRange()
        {
            // ab?[a-c]
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                CharacterWildcard.Default,
                new CharacterSet("a-c", false)
            });

            Assert.True(list.MatchesSegment("abca"));
            Assert.True(list.MatchesSegment("abda"));
            Assert.True(list.MatchesSegment("ab1a"));

            Assert.True(list.MatchesSegment("abcb"));
            Assert.True(list.MatchesSegment("abdb"));
            Assert.True(list.MatchesSegment("ab1b"));

            Assert.True(list.MatchesSegment("abcc"));
            Assert.True(list.MatchesSegment("abdc"));
            Assert.True(list.MatchesSegment("ab1c"));
        }

        [Fact]
        public void ShouldMatchCharacterSetInverted()
        {
            // ab?[!abc]
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                CharacterWildcard.Default,
                new CharacterSet("abc", true)
            });

            Assert.True(list.MatchesSegment("abcd"));
            Assert.True(list.MatchesSegment("abdd"));
            Assert.True(list.MatchesSegment("ab1d"));
            Assert.True(list.MatchesSegment("abce"));
            Assert.True(list.MatchesSegment("abde"));
            Assert.True(list.MatchesSegment("ab1e"));
            Assert.True(list.MatchesSegment("abcf"));
            Assert.True(list.MatchesSegment("abdf"));
            Assert.True(list.MatchesSegment("ab1f"));

            Assert.False(list.MatchesSegment("abca"));
            Assert.False(list.MatchesSegment("abda"));
            Assert.False(list.MatchesSegment("ab1a"));

            Assert.False(list.MatchesSegment("abcb"));
            Assert.False(list.MatchesSegment("abdb"));
            Assert.False(list.MatchesSegment("ab1b"));

            Assert.False(list.MatchesSegment("abcc"));
            Assert.False(list.MatchesSegment("abdc"));
            Assert.False(list.MatchesSegment("ab1c"));
        }
    }
}
