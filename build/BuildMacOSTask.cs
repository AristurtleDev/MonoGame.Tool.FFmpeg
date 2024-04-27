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
        // var isaarch64 = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.aarch64;

        // if (context.IsUniversalBinary || !isaarch64)
        // {
        //     BuildOsxX64(context);
        // }

        // if (context.IsUniversalBinary || isaarch64)
        // {
        // BuildOsxAarch64(context);
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
        var buildFlag = GetBuildConfigure(context);
        var hostFlag = GetHostConfigure(PlatformFamily.OSX);

        BuildOgg(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildVorbis(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildLame(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildFFMpeg(context, bashCommand, processSettings, ffmpegConfigureFlags);
    }

    private static void BuildOsxAarch64(BuildContext context)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        var env = new Dictionary<string, string>
        {
            {"CFLAGS", $"-fPIC -O3  -isysroot /Library/Developer/CommandLineTools/SDKs/MacOSX13.1.sdk -I{artifactsDir}/include -I/usr/local/opt/llvm@15/include"},
            {"CXXFLAGS", "-arch arm64"},
            {"CPPFLAGS", "-I/usr/local/opt/llvm@15/include"},
            {"LDFLAGS", $"-L{artifactsDir}/lib -L/usr/local/opt/llvm@15/lib/c++ -Wl,-rpath,/usr/local/opt/llvm@15/lib/c++ -arch arm64"},
            // {"CFLAGS", $"/usr/local/opt/llvm@15/bin/clang  -arch aarch64 -w -I{artifactsDir}/include -I/usr/local/opt/llvm@15/include -I/usr/include"},
            // {"CXXFLAGS", "/usr/local/opt/llvm@15/bin/clang++ -arch aarch64"},
            // {"CPPFLAGS", "-I/usr/local/opt/llvm@15/include"},
            // {"LDFLAGS", $"-L{artifactsDir}/lib -L/usr/local/opt/llvm@15/lib -L/usr/lib"}
        };

        var bashCommand = @"zsh";
        var processSettings = new ProcessSettings() { EnvironmentVariables = env };
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "osx-aarch64");
        var buildFlag = GetBuildConfigure(context);
        var hostFlag = GetHostConfigure(PlatformFamily.OSX, isAarch64: true);

        BuildOgg(context, bashCommand, processSettings, buildFlag, hostFlag);
        // BuildVorbis(context, bashCommand, processSettings, buildFlag, hostFlag);
        // BuildLame(context, bashCommand, processSettings, buildFlag, hostFlag);
        // BuildFFMpeg(context, bashCommand, processSettings, ffmpegConfigureFlags);
    }
}
