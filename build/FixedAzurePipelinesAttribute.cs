using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using System.Linq;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class FixedAzurePipelinesAttribute : AzurePipelinesAttribute
{
    public FixedAzurePipelinesAttribute([CanBeNull] string suffix, AzurePipelinesImage image, params AzurePipelinesImage[] images)
        : base(suffix, image, images)
    {
    }

    protected override AzurePipelinesJob GetJob(ExecutableTarget executableTarget, LookupTable<ExecutableTarget, AzurePipelinesJob> jobs)
    {
        var job = base.GetJob(executableTarget, jobs);

        var downloads = (from dep in job.Dependencies
            from pub in dep.PublishArtifacts
            select pub).ToArray();

        return new FixedAzurePipelinesJob
        {
            Name = job.Name,
            DisplayName = job.DisplayName,
            BuildCmdPath = job.BuildCmdPath,
            Dependencies = job.Dependencies,
            Parallel = job.Parallel,
            PartitionName = job.PartitionName,
            InvokedTargets = job.InvokedTargets,
            PublishArtifacts = job.PublishArtifacts,
            DownloadArtifacts = downloads
        };
    }
}

class FixedAzurePipelinesJob : AzurePipelinesJob {
    protected override void WriteSteps(CustomFileWriter writer)
    {
        DownloadArtifacts.ForEach(x =>
        {
            using (writer.WriteBlock("- task: DownloadBuildArtifacts@0"))
            {
                writer.WriteLine("displayName: Download Artifacts");
                using (writer.WriteBlock("inputs:"))
                {
                    string[] parts = x.Split('/');
                    var path = parts.SkipLast(1).Join('/').SingleQuote();
                    writer.WriteLine("buildType: 'current'");
                    writer.WriteLine("downloadType: 'single'");
                    writer.WriteLine($"artifactName: {parts.Last()}");
                    writer.WriteLine($"downloadPath: {path}");
                }
            }
        });

        base.WriteSteps(writer);
    }
}
