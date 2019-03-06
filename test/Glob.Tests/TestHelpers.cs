using System;
using System.IO;
using System.Text;
using Microsoft.DotNet.PlatformAbstractions;

namespace GlobExpressions.Tests
{
    public static class TestHelpers
    {
        public static readonly string SourceRoot = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER") ?? Path.Combine("..", "..", "..", "..", "..");
        public static readonly string FileSystemRoot =
            RuntimeEnvironment.OperatingSystemPlatform == Platform.Windows ? "c:\\" : "/";
    }
}
