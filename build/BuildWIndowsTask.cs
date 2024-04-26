using System.Diagnostics;

namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        var env = new Dictionary<string, string>
        {
            {"PATH", "/usr/bin:/mingw64/bin:$PATH"},
            {"CC", "x86_64-w64-mingw32-gcc"},
            {"CFLAGS", $"-I{artifactsDir}/include -I/mingw64/include -I/usr/include "},
            {"CXXFLAGS", "-static"},
            {"LDFLAGS", $"-L{artifactsDir}/lib -L/mingw64/lib -L/usr/lib  --static"}
        };

        var bashCommand = @"C:\msys64\usr\bin\bash";
        var processSettings = new ProcessSettings() { EnvironmentVariables = env };
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "windows-x64");
        var buildFlag = GetBuildConfigure(context);
        var hostFlag = GetHostConfigure(PlatformFamily.Windows);

        BuildOgg(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildVorbis(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildLame(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildFFMpeg(context, bashCommand, processSettings, ffmpegConfigureFlags);
    }
}
