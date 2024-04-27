using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace BuildScripts;

[TaskName("Build libvorbis")]
public sealed class BuildVorbisTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.IsRunningOnWindows())
            WindowsBuild(context);
        else if (context.IsRunningOnLinux())
            LinuxBuild(context);
        else if (context.IsRunningOnMacOs())
            MacBuild(context);
        else
            throw new PlatformNotSupportedException("Unsupported Platform");
    }

    private static void WindowsBuild(BuildContext context)
    {
        throw new NotImplementedException();
    }

    private static void LinuxBuild(BuildContext context)
    {
        throw new NotImplementedException();
    }

    private static void MacBuild(BuildContext context)
    {
        var absoluteArtifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));
        if (context.IsUniversalBinary || RuntimeInformation.ProcessArchitecture is Architecture.Arm or Architecture.Arm64)
        {
            var buildSettings = new BuildSettings
            (
                WorkingDirectory: "./vorbis",
                ShellExecutionPath: "zsh",
                CFLags: $"-arch arm64 -I{absoluteArtifactDir}/osx-arm64/include",
                CXXFlags: "-arch arm64",
                CPPFlags: $"-arch arm64 -I{absoluteArtifactDir}/osx-arm64/include",
                LDFLags: $"-arch arm64 -L{absoluteArtifactDir}/osx-arm64/lib",
                Prefix: $"{absoluteArtifactDir}/osx-arm64",
                Host: "aarch64-apple-darwin"
            );

            BuildVorbis(context, buildSettings);
        }

        if (context.IsUniversalBinary || RuntimeInformation.ProcessArchitecture is Architecture.X64)
        {
            var buildSettings = new BuildSettings
            (
                WorkingDirectory: "./vorbis",
                ShellExecutionPath: "zsh",
                CFLags: $"-arch x86_64 -I{absoluteArtifactDir}/osx-x86_64/include",
                CXXFlags: "-arch x86_64",
                CPPFlags: $"-arch x86_64 -I{absoluteArtifactDir}/osx-x86_64/include",
                LDFLags: $"-arch x86_64 -L{absoluteArtifactDir}/osx-x86_64/lib",
                Prefix: $"{absoluteArtifactDir}/osx-x86_64",
                Host: "x86_64-apple-darwin"
            );

            BuildVorbis(context, buildSettings);
        }
    }

    private static void BuildVorbis(BuildContext context, BuildSettings buildSettings)
    {
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = buildSettings.WorkingDirectory,
            EnvironmentVariables = new Dictionary<string, string>
            {
                {"CFLAGS", buildSettings.CFLags},
                {"CXXFLAGS", buildSettings.CXXFlags},
                {"CPPFLAGS", buildSettings.CPPFlags},
                {"LDFLAGS", buildSettings.LDFLags}
            }
        };

        //  Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(buildSettings.ShellExecutionPath, processSettings);

        //  Run autogen.sh to create configuration files
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(buildSettings.ShellExecutionPath, processSettings);

        //  Run configure to build make file
        processSettings.Arguments = $"-c \"./configure --prefix=\"{buildSettings.Prefix}\" --host=\"{buildSettings.Host}\" --disable-examples --disable-docs --disable-shared";
        context.StartProcess(buildSettings.ShellExecutionPath, processSettings);

        //  Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(buildSettings.ShellExecutionPath, processSettings);

        //  Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(buildSettings.ShellExecutionPath, processSettings);
    }
}
