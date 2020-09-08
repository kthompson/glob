using System;
using System.IO;
using System.Text;
using Microsoft.DotNet.PlatformAbstractions;

namespace GlobExpressions.Tests
{
    public static class TestHelpers
    {
        private static readonly string? NCrunchSourceRoot = Environment.GetEnvironmentVariable("NCrunch") == "1"
            ? Path.GetDirectoryName(Environment.GetEnvironmentVariable("NCrunch.OriginalSolutionPath"))
            : null;

        public static readonly string SourceRoot = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER")
                                                   ?? NCrunchSourceRoot
                                                   ?? Path.Combine("..", "..", "..", "..", "..");

        public static readonly string FileSystemRoot =
            RuntimeEnvironment.OperatingSystemPlatform == Platform.Windows ? "c:\\" : "/";

    }
}
