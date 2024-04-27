namespace BuildScripts;

public class BuildSettings
{
    public string WorkingDirectory {get; set;} = string.Empty;
    public string ShellExecutionPath {get; set;} = string.Empty;
    public string CFlags {get; set; } = string.Empty;
    public string CXXFlags {get; set;} = string.Empty;
    public string CPPFlags {get; set;} = string.Empty;
    public string LDFlags  {get; set;} = string.Empty;
    public string PrefixFlag {get; set;} = string.Empty;
    public string HostFlag {get; set;} = string.Empty;
}