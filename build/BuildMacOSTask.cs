using System.Runtime.InteropServices;

namespace BuildScripts;

[TaskName("Build macOS")]
[IsDependentOn(typeof(PrepTask))]
[IsDependentOn(typeof(BuildOggTask))]
[IsDependentOn(typeof(BuildVorbisTask))]
[IsDependentOn(typeof(BuildLameTask))]
[IsDependentOn(typeof(BuildFFMpegTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildMacOSTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnMacOs();

    public override void Run(BuildContext context)
    {
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));
        var x8664BinaryPath = $"{absoluteArtifactDir}/osx-x86_64/bin/ffmpeg";
        var arm64BinaryPath = $"{absoluteArtifactDir}/osx-arm64/bin/ffmpeg";

        if (context.IsUniversalBinary)
        {
            // If building a universal binary, after the dependent tasks finish building hte x86_64 and arm64 binaries, 
            // we'll need to combine them into a single binary using lipo.
            context.StartProcess("lipo", new ProcessSettings()
            {
                Arguments = $"-create {x8664BinaryPath} {arm64BinaryPath} -output {absoluteArtifactDir}/ffmpeg"
            });
        }
        else if (RuntimeInformation.ProcessArchitecture is Architecture.Arm or Architecture.Arm64)
        {
            context.CopyFile(arm64BinaryPath, $"{absoluteArtifactDir}/ffmpeg");
        }
        else
        {
            context.CopyFile(x8664BinaryPath, $"{absoluteArtifactDir}/ffmpeg");
        }
    }
}
