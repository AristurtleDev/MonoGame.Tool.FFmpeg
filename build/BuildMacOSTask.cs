using System.Runtime.InteropServices;

namespace BuildScripts;

[TaskName("Build macOS")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildMacOSTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnMacOs();

    public override void Run(BuildContext context)
    {
        // var isArm64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        // if (context.IsUniversalBinary || !isArm64)
        // {
        //     BuildOsxX64(context);
        // }

        // if (context.IsUniversalBinary || isArm64)
        // {
            BuildOsxArm64(context);
        // }
    }

    private static void BuildOsxX64(BuildContext context)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        var env = new Dictionary<string, string>
        {
            {"CFLAGS", $"-w -I{artifactsDir}/include -I/usr/include"},
            {"CXXFLAGS", ""},
            {"LDFLAGS", $"-L{artifactsDir}/lib -L/usr/lib"}
        };

        var bashCommand = @"zsh";
        var processSettings = new ProcessSettings() { EnvironmentVariables = env };
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-x64");

        BuildOgg(context, bashCommand, processSettings);
        BuildVorbis(context, bashCommand, processSettings);
        BuildLame(context, bashCommand, processSettings);
        BuildFFMpeg(context, bashCommand, processSettings, ffmpegConfigureFlags);
    }

    private static void BuildOsxArm64(BuildContext context)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        var env = new Dictionary<string, string>
        {
            {"CFLAGS", $"-w -arch=arm64 -I{artifactsDir}/include -I/usr/include"},
            {"CXXFLAGS", "-arch=arm64"},
            {"LDFLAGS", $"-L{artifactsDir}/lib -L/usr/lib"}
        };

        var bashCommand = @"zsh";
        var processSettings = new ProcessSettings() { EnvironmentVariables = env };
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-arm64");

        BuildOgg(context, bashCommand, processSettings);
        BuildVorbis(context, bashCommand, processSettings);
        BuildLame(context, bashCommand, processSettings);
        BuildFFMpeg(context, bashCommand, processSettings, ffmpegConfigureFlags);
    }
}
