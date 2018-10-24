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

            Assert.True(list.MatchesSegment("", false));
            Assert.True(list.MatchesSegment("a", false));
            Assert.True(list.MatchesSegment("abc", false));
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

            Assert.True(list.MatchesSegment("abcd", false));
            Assert.True(list.MatchesSegment("abcdcd", false));
            Assert.True(list.MatchesSegment("ab123456cd", false));

            Assert.False(list.MatchesSegment("ab123456cd11", false));
            Assert.False(list.MatchesSegment("abcd1", false));
            Assert.False(list.MatchesSegment("abcdcd1", false));
            Assert.False(list.MatchesSegment("ab123456cd1", false));
        }

        [Fact]
        public void ShouldMatchLiteralSet()
        {
            // ab*{cd,ef}
            var list = new DirectorySegment(new SubSegment[]
            {
                new Identifier("ab"),
                StringWildcard.Default,
                new LiteralSet("cd", "ef"),
            });

            Assert.True(list.MatchesSegment("abcd", false));
            Assert.True(list.MatchesSegment("abcdcd", false));
            Assert.True(list.MatchesSegment("ab123456cd", false));

            Assert.True(list.MatchesSegment("abef", false));
            Assert.True(list.MatchesSegment("abcdef", false));
            Assert.True(list.MatchesSegment("ab123456ef", false));

            Assert.False(list.MatchesSegment("ab123456cd11", false));
            Assert.False(list.MatchesSegment("abcd1", false));
            Assert.False(list.MatchesSegment("abcdcd1", false));
            Assert.False(list.MatchesSegment("ab123456cd1", false));
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

            Assert.True(list.MatchesSegment("abc", false));
            Assert.True(list.MatchesSegment("abd", false));
            Assert.True(list.MatchesSegment("ab1", false));

            Assert.False(list.MatchesSegment("eab", false));
            Assert.False(list.MatchesSegment("abef", false));
            Assert.False(list.MatchesSegment("ab123456cd11", false));
            Assert.False(list.MatchesSegment("abcd1", false));
            Assert.False(list.MatchesSegment("abcdcd1", false));
            Assert.False(list.MatchesSegment("ab123456cd1", false));
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

            Assert.True(list.MatchesSegment("abca", false));
            Assert.True(list.MatchesSegment("abda", false));
            Assert.True(list.MatchesSegment("ab1a", false));

            Assert.True(list.MatchesSegment("abcb", false));
            Assert.True(list.MatchesSegment("abdb", false));
            Assert.True(list.MatchesSegment("ab1b", false));

            Assert.True(list.MatchesSegment("abcc", false));
            Assert.True(list.MatchesSegment("abdc", false));
            Assert.True(list.MatchesSegment("ab1c", false));
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

            Assert.True(list.MatchesSegment("abca", false));
            Assert.True(list.MatchesSegment("abda", false));
            Assert.True(list.MatchesSegment("ab1a", false));

            Assert.True(list.MatchesSegment("abcb", false));
            Assert.True(list.MatchesSegment("abdb", false));
            Assert.True(list.MatchesSegment("ab1b", false));

            Assert.True(list.MatchesSegment("abcc", false));
            Assert.True(list.MatchesSegment("abdc", false));
            Assert.True(list.MatchesSegment("ab1c", false));
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

            Assert.True(list.MatchesSegment("abcd", false));
            Assert.True(list.MatchesSegment("abdd", false));
            Assert.True(list.MatchesSegment("ab1d", false));
            Assert.True(list.MatchesSegment("abce", false));
            Assert.True(list.MatchesSegment("abde", false));
            Assert.True(list.MatchesSegment("ab1e", false));
            Assert.True(list.MatchesSegment("abcf", false));
            Assert.True(list.MatchesSegment("abdf", false));
            Assert.True(list.MatchesSegment("ab1f", false));

            Assert.False(list.MatchesSegment("abca", false));
            Assert.False(list.MatchesSegment("abda", false));
            Assert.False(list.MatchesSegment("ab1a", false));

            Assert.False(list.MatchesSegment("abcb", false));
            Assert.False(list.MatchesSegment("abdb", false));
            Assert.False(list.MatchesSegment("ab1b", false));

            Assert.False(list.MatchesSegment("abcc", false));
            Assert.False(list.MatchesSegment("abdc", false));
            Assert.False(list.MatchesSegment("ab1c", false));
        }
    }
}
