namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildLinuxTask : BuildTaskBase
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        // Absolute path to the artifact directory is needed for flags since they don't allow relative path
        var artifactDir = context.MakeAbsolute(new DirectoryPath(context.ArtifactsDir));
        var dependencyDir = context.MakeAbsolute(new DirectoryPath($"{context.ArtifactsDir}/../dependencies-linux-x64"));
        var prefixFlag = $"--prefix=\"{dependencyDir}\"";
        var hostFlag = "--host=\"x86_64-linux-gnu\"";
        var binDirFlag = $"--bindir=\"{artifactDir}\"";

        var envVariables = new Dictionary<string, string>
        {
            {"CFLAGS", $"-w -I{dependencyDir}/include"},
            {"CPPFLAGS", $"-I{dependencyDir}/include"},
            {"LDFLAGS", $"--static -L{dependencyDir}/lib"},
            {"PKG_CONFIG_PATH", $"{dependencyDir}/lib/pkgconfig"}
        };

        var configureFlags = GetFFMpegConfigureFlags(context, "linux-x64");
        var processSettings = new ProcessSettings();

        var shellCommandPath = "sh";

        // Build libogg
        processSettings.WorkingDirectory = "./ogg";
        processSettings.EnvironmentVariables = envVariables;
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"./configure --disable-shared {prefixFlag} {hostFlag}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(shellCommandPath, processSettings);

        // build libvorbis
        processSettings.WorkingDirectory = "./vorbis";
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"./configure --disable-examples --disable-docs --disable-shared {prefixFlag} {hostFlag}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(shellCommandPath, processSettings);

        // build lame
        processSettings.WorkingDirectory = "./lame";
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"./configure --disable-frontend --disable-decoder --disable-shared {prefixFlag} {hostFlag}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(shellCommandPath, processSettings);

        // Build ffmpeg
        processSettings.WorkingDirectory = "./ffmpeg";
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"./configure {binDirFlag} {configureFlags}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(shellCommandPath, processSettings);
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(shellCommandPath, processSettings);
    }
}
