namespace BuildScripts;

public class BuildSettings
{
    public string ShellExecutionPath { get; set; } = string.Empty;
    public string CFlags { get; set; } = string.Empty;
    public string CCFlags { get; set; } = string.Empty;
    public string CXXFlags { get; set; } = string.Empty;
    public string CPPFlags { get; set; } = string.Empty;
    public string LDFlags { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string PrefixFlag { get; set; } = string.Empty;
    public string HostFlag { get; set; } = string.Empty;

    public IDictionary<string, string> GetEnvironmentVariables()
    {
        var environmentVariables = new Dictionary<string, string>();

        // Only add those that have a value otherwise it could cause issues for builds.
        if (!string.IsNullOrEmpty(CFlags))
            environmentVariables.Add("CFLAGS", CFlags);

        if (!string.IsNullOrEmpty(CCFlags))
            environmentVariables.Add("CCFLAGS", CCFlags);

        if (!string.IsNullOrEmpty(CXXFlags))
            environmentVariables.Add("CXXFLAGS", CXXFlags);

        if (!string.IsNullOrEmpty(CPPFlags))
            environmentVariables.Add("CPPFLAGS", CPPFlags);

        if (!string.IsNullOrEmpty(LDFlags))
            environmentVariables.Add("LDFLAGS", LDFlags);

        if (!string.IsNullOrEmpty(Path))
            environmentVariables.Add("PATH", $"{Path}:$PATH");

        return environmentVariables;
    }
}