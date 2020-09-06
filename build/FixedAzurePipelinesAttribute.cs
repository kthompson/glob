using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Utilities;
using Nuke.Common;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class FixedAzurePipelinesAttribute : AzurePipelinesAttribute
{
    public FixedAzurePipelinesAttribute([CanBeNull] string suffix, AzurePipelinesImage image, params AzurePipelinesImage[] images)
        : base(suffix, image, images)
    {
    }

    public override ConfigurationEntity GetConfiguration(NukeBuild build, IReadOnlyCollection<ExecutableTarget> relevantTargets) =>
        FixStageJobs((AzurePipelinesConfiguration)base.GetConfiguration(build, relevantTargets));

    static AzurePipelinesConfiguration FixStageJobs(AzurePipelinesConfiguration config)
    {
        config.Stages = config.Stages
            .Select(stage => stage.Name != "windows_latest" ? RemovePublishAndPackJobs(stage) : stage)
            .ToArray();

        return config;
    }

    static AzurePipelinesStage RemovePublishAndPackJobs(AzurePipelinesStage stage)
    {
        stage.Jobs = stage.Jobs.Where(job => job.Name != "Publish" && job.Name != "Pack").ToArray();
        return stage;
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
            DownloadArtifacts = downloads,
            ExtraArgs = job.Name == "Publish" ? " --api-key $(ApiKey)" : ""
        };
    }
}

class FixedAzurePipelinesJob : AzurePipelinesJob {
    public string ExtraArgs { get; set; }

    protected override void WriteSteps(CustomFileWriter writer)
    {
        DownloadArtifacts.ForEach(x =>
        {
            using (AzurePipelinesCustomWriterExtensions.WriteBlock(writer, "- task: DownloadBuildArtifacts@0"))
            {
                writer.WriteLine("displayName: Download Artifacts");
                using (AzurePipelinesCustomWriterExtensions.WriteBlock(writer, "inputs:"))
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

        using (writer.WriteBlock("- task: CmdLine@2"))
        {
            var arguments = $"{InvokedTargets.JoinSpace()} --skip";
            if (PartitionName != null)
                arguments += $" --test-partition $(System.JobPositionInPhase)";

            using (writer.WriteBlock("inputs:"))
            {
                writer.WriteLine($"script: './{BuildCmdPath} {arguments}{ExtraArgs}'");
            }
        }

        PublishArtifacts.ForEach(x =>
        {
            using (writer.WriteBlock("- task: PublishBuildArtifacts@1"))
            {
                using (writer.WriteBlock("inputs:"))
                {
                    writer.WriteLine($"artifactName: {x.Split('/').Last()}");
                    writer.WriteLine($"pathtoPublish: {x.SingleQuote()}");
                }
            }
        });
    }
}
