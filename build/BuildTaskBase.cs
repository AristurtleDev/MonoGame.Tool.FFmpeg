namespace BuildScripts;

public abstract class BuildTaskBase : FrostingTask<BuildContext>
{
    protected static void BuildOgg(BuildContext context, string command, ProcessSettings processSettings)
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
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}/bin\" --enable-static --disable-shared\"";
        context.StartProcess(command, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(command, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(command, processSettings);
    }

    protected static void BuildVorbis(BuildContext context, string command, ProcessSettings processSettings)
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
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}/bin\" --enable-static --host=x86_64-w64-mingw32\"";
        context.StartProcess(command, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j{Environment.ProcessorCount}\"";
        context.StartProcess(command, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(command, processSettings);
    }

    protected static void BuildLame(BuildContext context, string command, ProcessSettings processSettings)
    {
        var artifactsDir = GetFullPathToArtifactDirectory(context);
        processSettings.WorkingDirectory = "./lame";

        //  Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(command, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}/bin\" --enable-static --disable-frontend --disable-decoder --host=x86_64-w64-mingw32\"";
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
            fullPath = $"/{fullPath[0]}{fullPath[2]}";
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
}
