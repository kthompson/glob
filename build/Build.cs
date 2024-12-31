using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;

[GitHubActions(
    "pr",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    FetchDepth = 0,
    OnPullRequestBranches = new[] { DevelopBranch },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(ITest.Test), nameof(IReportCoverage.ReportCoverage), nameof(IPack.Pack) },
    CacheKeyFiles = new[] { "global.json", "**/*.csproj" },
    ImportSecrets = new[] { nameof(IReportCoverage.CodecovToken) },
    EnableGitHubToken = true)]
[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = new[] { MainBranch, DevelopBranch },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(IReportCoverage.ReportCoverage), nameof(IPublish.Publish) },
    CacheKeyFiles = new[] { "global.json", "**/*.csproj" },
    ImportSecrets = new[] { nameof(PublicNuGetApiKey) },
    EnableGitHubToken = true)]
class Build : NukeBuild,
    IHazChangelog,
    IHazGitRepository,
    IHazNerdbankGitVersioning,
    IHazSolution,
    IRestore,
    ICompile,
    IPack,
    ITest,
    IReportCoverage,
    IReportIssues,
    IReportDuplicates,
    IPublish
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => ((IPack)x).Pack);

    [CI] readonly GitHubActions GitHubActions;

    GitRepository GitRepository => From<IHazGitRepository>().GitRepository;
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";

    const string MainBranch = "main";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";
    const string HotfixBranchPrefix = "hotfix";

    Target Clean => _ => _
        .Before<IRestore>()
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            From<IHazArtifacts>().ArtifactsDirectory.CreateOrCleanDirectory();
        });

    public IEnumerable<Project> TestProjects => From<IHazSolution>().Solution.GetAllProjects("*.Tests");

    public bool CreateCoverageHtmlReport => true;
    public bool ReportToCodecov => false; // TODO: #74 RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public Configure<ReportGeneratorSettings> ReportGeneratorSettings => x => x.SetFramework("net8.0");

    string PublicNuGetSource => "https://api.nuget.org/v3/index.json";

    string GitHubRegistrySource => GitHubActions != null
        ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
        : null;


    [Parameter][Secret] readonly string PublicNuGetApiKey;

    bool IsOriginalRepository => GitRepository.Identifier == "kthompson/glob";
    string IPublish.NuGetApiKey => GitRepository.IsOnMainBranch() ? PublicNuGetApiKey : GitHubActions.Token;
    string IPublish.NuGetSource => GitRepository.IsOnMainBranch() ? PublicNuGetSource : GitHubRegistrySource;


    Target IPublish.Publish => _ => _
        .Inherit<IPublish>()
        .Consumes(From<IPack>().Pack)
        .Requires(() =>
            IsOriginalRepository && GitHubActions != null &&
            (GitRepository.IsOnMainBranch() || GitRepository.IsOnDevelopBranch()))
        .WhenSkipped(DependencyBehavior.Execute);

    T From<T>()
        where T : INukeBuild
        => (T)(object)this;
}
