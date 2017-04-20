using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Glob.Tests
{
    public class ScannerTests
    {
        [Fact]
        public void CanParseSimpleFilename()
        {
            var scanner = new Scanner("file.*");
            AssertToken(TokenKind.Identifier, "file.", scanner.Scan());
            AssertToken(TokenKind.Wildcard, "*", scanner.Scan());
            AssertToken(TokenKind.EOT, "", scanner.Scan());
        }

        [Fact]
        public void CanParseSimpleFilename2()
        {
            var scanner = new Scanner("*.txt");
            AssertToken(TokenKind.Wildcard, "*", scanner.Scan());
            AssertToken(TokenKind.Identifier, ".txt", scanner.Scan());
            AssertToken(TokenKind.EOT, "", scanner.Scan());
        }

        [Fact]
        public void CanParseFileNameWithUnderscore()
        {
            var scanner = new Scanner("_foo_bar.*");
            AssertToken( TokenKind.Identifier, "_foo_bar.", scanner.Scan() );
            AssertToken( TokenKind.Wildcard, "*", scanner.Scan() );
            AssertToken( TokenKind.EOT, "", scanner.Scan() );
        }

        [Fact]
        public void ParseWindowsRoot()
        {
            var scanner = new Scanner(@"C:\*.txt");
            AssertToken(TokenKind.WindowsRoot, "C:", scanner.Scan());
            AssertToken(TokenKind.PathSeperator, "\\", scanner.Scan());
            AssertToken(TokenKind.Wildcard, "*", scanner.Scan());
            AssertToken(TokenKind.Identifier, ".txt", scanner.Scan());
            AssertToken(TokenKind.EOT, "", scanner.Scan());
        }

        private static void AssertToken(TokenKind kind, string spelling, Token token)
        {
            Assert.Equal(kind, token.Kind);
            Assert.Equal(spelling, token.Spelling);
        }
    }
}
