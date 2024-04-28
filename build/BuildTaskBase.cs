using System.Runtime.InteropServices;

namespace BuildScripts;

public abstract class BuildTaskBase : FrostingTask<BuildContext>
{
    protected static void BuildOgg(BuildContext context, BuildSettings buildSettings)
    {
        context.Information(" ");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information("Building OGG");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information(" ");
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./ogg",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = BuildArgument(buildSettings, "make distclean");
        // processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run autogen.sh to create configuration files
        processSettings.Arguments = BuildArgument(buildSettings, "./autogen.sh");
        // processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run configure to build make file
        processSettings.Arguments = BuildArgument(buildSettings, $"./configure --prefix=\"{buildSettings.PrefixFlag}\" --host=\"{buildSettings.HostFlag}\" --disable-shared");
        // processSettings.Arguments = $"-c \"./configure --prefix=\"{buildSettings.PrefixFlag}\" --host=\"{buildSettings.HostFlag}\" --disable-shared";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make
        processSettings.Arguments = BuildArgument(buildSettings, $"make -j{Environment.ProcessorCount}");
        // processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make install
        processSettings.Arguments = BuildArgument(buildSettings, "make install");
        // processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);
    }

    protected static void BuildVorbis(BuildContext context, BuildSettings buildSettings)
    {
        context.Information(" ");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information("Building Vorbis");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information(" ");
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./vorbis",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run autogen.sh to create configuration files
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run configure to build make file
        processSettings.Arguments = $"-c \"./configure --prefix=\"{buildSettings.PrefixFlag}\" --host=\"{buildSettings.HostFlag}\" --disable-examples --disable-docs --disable-shared";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);
    }

    protected static void BuildLame(BuildContext context, BuildSettings buildSettings)
    {
        context.Information(" ");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information("Building Lame");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information(" ");
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./lame",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run configure to build make file
        processSettings.Arguments = $"-c \"./configure --prefix='{buildSettings.PrefixFlag}' --host=\"{buildSettings.HostFlag}\" --disable-frontend --disable-decoder --disable-shared\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);
    }

    protected static void BuildFFMpeg(BuildContext context, BuildSettings buildSettings, string configureFlags)
    {
        context.Information(" ");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information("Building FFMpeg");
        context.Information("++++++++++++++++++++++++++++++++++++++++++++++++++");
        context.Information(" ");
        var processSettings = new ProcessSettings()
        {
            WorkingDirectory = "./ffmpeg",
            EnvironmentVariables = buildSettings.GetEnvironmentVariables()
        };

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run configure to build make file
        processSettings.Arguments = $"-c \"./configure --prefix=\"{buildSettings.PrefixFlag}\" {configureFlags}";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(buildSettings.ShellCommand, processSettings);
    }

    protected static string GetFullPathToArtifactDirectory(BuildContext context)
    {
        string fullPath = System.IO.Path.GetFullPath(context.ArtifactsDir);

        if (context.IsRunningOnWindows())
        {
            // Windows uses mingw for compilation and expects paths to be in unix format
            // e.g. C:\Users\MonoGame\Desktop\ => /c/Users/MonoGame/Desktop
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

    protected static string BuildArgument(BuildSettings buildSettings, string argument)
    {
        var pathEnv = string.IsNullOrEmpty(buildSettings.Path)
                      ? string.Empty
                      : $"export PATH=\"{buildSettings.Path}:$PATH;\"";
        return $"-c \"{pathEnv}{argument}\"";
    }
}
