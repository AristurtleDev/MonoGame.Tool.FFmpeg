using System.Runtime.InteropServices;

namespace BuildScripts;

public abstract class BuildTaskBase : FrostingTask<BuildContext>
{
    protected static void BuildOgg(BuildContext context, string command, ProcessSettings processSettings, string buildFlagValue, string hostFlagValue)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        processSettings.WorkingDirectory = "./ogg";

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(command, processSettings);

        // Run autogen.sh
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(command, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --build={buildFlagValue} --host={hostFlagValue}\"";
        context.StartProcess(command, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(command, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(command, processSettings);
    }

    protected static void BuildVorbis(BuildContext context, string command, ProcessSettings processSettings, string buildFlagValue, string hostFlagValue)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        processSettings.WorkingDirectory = "./vorbis";

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c make distclean\"";
        context.StartProcess(command, processSettings);

        // Run autogen.sh
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(command, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --build={buildFlagValue} --host={hostFlagValue} --disable-docs --disable-exmaples";
        context.StartProcess(command, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(command, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(command, processSettings);
    }

    protected static void BuildLame(BuildContext context, string command, ProcessSettings processSettings, string buildFlagValue, string hostFlagValue)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        processSettings.WorkingDirectory = "./lame";

        //  Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(command, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --build={buildFlagValue} --host={hostFlagValue} --disable-frontend --disable-decoder\"";
        context.StartProcess(command, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(command, processSettings);

        //  Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(command, processSettings);
    }

    protected static void BuildFFMpeg(BuildContext context, string command, ProcessSettings processSettings, string configureFlags)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        processSettings.WorkingDirectory = "./ffmpeg";

        //  Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(command, processSettings);

        // Run configure
        configureFlags = $"--prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}\" {configureFlags}";
        context.Information($"Building Options: {configureFlags}");
        processSettings.Arguments = $"-c \"./configure {configureFlags}\"";
        context.StartProcess(command, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(command, processSettings);

        //  Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(command, processSettings);
    }

    protected static string GetFullPathToArtifactDirectory(BuildContext context)
    {
        string fullPath = System.IO.Path.GetFullPath(context.ArtifactsDir);

        if (context.IsRunningOnWindows())
        {
            // Windows uses mingw for compilation and expects paths to be in unix format
            //  e.g. C:\Users\MonoGame\Desktop\ => /c/Users/MonoGame/Desktop
            fullPath = fullPath.Replace("\\", "/");
            fullPath = $"/{fullPath[0]}{fullPath[2..]}";
        }

        return fullPath;
    }

    protected static string GetFFMpegConfigureFlags(BuildContext context, string rid)
    {
        var ignoreCommentsAndNewLines = (string line) => !line.StartsWith('#') && !line.StartsWith(' ');
        var configureFlags = context.FileReadLines("ffmpeg.config").Where(ignoreCommentsAndNewLines);
        var osConfigureFlags = context.FileReadLines($"ffmpeg.{rid}.config").Where(ignoreCommentsAndNewLines);
        return string.Join(' ', configureFlags) + " " + string.Join(' ', osConfigureFlags);
    }

    protected static string GetBuildConfigure(BuildContext context) => context switch
    {
        _ when context.IsRunningOnWindows() => "x86_64-w64-mingw32",
        _ when context.IsRunningOnLinux() => "x86_64-linux-gnu",
        _ when context.IsRunningOnMacOs() => RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.Arm or Architecture.Arm64 => "aarch64-apple-darwin",
            _ => "x86_64-apple-darwin"
        },
        _ => throw new PlatformNotSupportedException("Unsupported Platform")
    };

    protected static string GetHostConfigure(PlatformFamily platform, bool isAarch64 = false) => platform switch
    {
        PlatformFamily.Windows => "x86_64-w64-mingw32",
        PlatformFamily.Linux => "x86_64-linux-gnu",
        PlatformFamily.OSX => isAarch64 switch
        {
            true => "aarch64-apple-darwin",
            _ => "x86_64-apple-darwin"
        },
        _ => throw new PlatformNotSupportedException("Unsupported Platform")
    };



}
