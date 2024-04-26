namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildLinuxTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);

        var env = new Dictionary<string, string>
        {
            {"CFLAGS", $"-w -I/mingw64/include -I/usr/include -I{artifactsDir}/include"},
            {"CXXFLAGS", "-static"},
            {"LDFLAGS", $"-L/mingw64/lib -L/usr/lib -L{artifactsDir}/lib --static"}
        };

        var bashCommand = "sh";
        var processSettings = new ProcessSettings() { EnvironmentVariables = env };
        var ffmpegConfigureFlags = GetFFMpegConfigureFlags(context, "linux-x64");
        var buildFlag = GetBuildConfigure(context);
        var hostFlag = GetHostConfigure(PlatformFamily.Linux);

        BuildOgg(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildVorbis(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildLame(context, bashCommand, processSettings, buildFlag, hostFlag);
        BuildFFMpeg(context, bashCommand, processSettings, ffmpegConfigureFlags);
    }
}
