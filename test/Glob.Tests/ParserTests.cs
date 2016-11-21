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

        }
    }
}