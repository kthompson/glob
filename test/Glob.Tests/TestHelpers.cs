using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GlobExpressions.Tests
{
    public static class TestHelpers
    {
        public static readonly string SourceRoot = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER") ?? Path.Combine("..", "..", "..", "..", "..");
    }
}
