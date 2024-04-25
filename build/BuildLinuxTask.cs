namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildLinuxTask : FrostingTask<BuildContext>
{
    private const string BASH_FILE_PATH = @"sh";

    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        // The include and lib files generated from the build will be output into the artifacts directory, however, the
        // --prefix flag needs an absolute path not a relative path
        var artifactsDirFullPath = System.IO.Path.GetFullPath(context.ArtifactsDir);

        var env = new Dictionary<string, string>
        {
            {"CFLAGS", $"-w -I/mingw64/include -I/usr/include -I{artifactsDirFullPath}/include"},
            {"CXXFLAGS", "-static -lpthread"},
            {"LDFLAGS", $"-L/mingw64/lib -L/usr/lib -L{artifactsDirFullPath}/lib --static"}
        };
        var processSettings = new ProcessSettings() { EnvironmentVariables = env };

        BuildVorbis(context, processSettings, artifactsDirFullPath);
        BuildLame(context, processSettings, artifactsDirFullPath);
        BuildFFMpeg(context, processSettings, artifactsDirFullPath);
    }

    private static void BuildVorbis(BuildContext context, ProcessSettings processSettings, string artifactsDir)
    {
        processSettings.WorkingDirectory = "./vorbis";

        // Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run autogen.sh
        processSettings.Arguments = $"-c \"./autogen.sh\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}/bin\" --enable-static\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j$(nproc)\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);
    }

    private static void BuildLame(BuildContext context, ProcessSettings processSettings, string artifactsDir)
    {
        processSettings.WorkingDirectory = "./lame";

        //  Ensure clean start if we're running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}/bin\" --enable-static --disable-frontend --disable-decoder\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j$(nproc)\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        //  Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);
    }

    private static void BuildFFMpeg(BuildContext context, ProcessSettings processSettings, string artifactsDir)
    {
        processSettings.WorkingDirectory = "./ffmpeg";

        //  Ensure clean start if we\"re running locally and testing over and over
        processSettings.Arguments = $"-c \"make distclean\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run configure
        processSettings.Arguments = $"-c \"./configure --prefix=\"{artifactsDir}\" --bindir=\"{artifactsDir}/artifacts-linux-x64\" --disable-encoders --disable-muxers --enable-encoder=pcm_u8 --enable-encoder=pcm_f32le --enable-encoder=pcm_s16le --enable-encoder=adpcm_ms --enable-encoder=wmav2 --enable-encoder=adpcm_ima_wav --enable-encoder=aac --enable-muxer=mp4 --enable-muxer=wav --enable-muxer=asf --enable-muxer=ipod --enable-muxer=ogg --enable-muxer=mp3 --enable-libvorbis --enable-libmp3lame --enable-pic --disable-autodetect --disable-doc --disable-network --disable-inline-asm --pkg-config-flags=\"--static\" --enable-static --enable-optimizations --enable-asm --enable-x86asm --arch=x86_64 --enable-ffmpeg --disable-ffprobe --disable-ffplay";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        // Run make
        processSettings.Arguments = $"-c \"make -j$(nproc)\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);

        //  Run make install
        processSettings.Arguments = $"-c \"make install\"";
        context.StartProcess(BASH_FILE_PATH, processSettings);
    }
}
