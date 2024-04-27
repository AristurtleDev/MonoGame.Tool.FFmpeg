namespace BuildScripts;

public record BuildSettings(string WorkingDirectory,
                            string ShellExecutionPath,
                            string CFLags,
                            string CXXFlags,
                            string CPPFlags,
                            string LDFLags,
                            string Prefix,
                            string Host);