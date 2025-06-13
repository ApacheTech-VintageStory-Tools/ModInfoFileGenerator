using CommandLine;

// ReSharper disable ClassNeverInstantiated.Global

namespace ModInfoFileGenerator
{
    public sealed class PackagerCommandLineArgs
    {
        [Option('a', "assembly",
            Required = true,
            MetaValue = "$(TargetPath)",
            HelpText = "The target assembly. In most cases, should be $(TargetPath) in MSBuild Macros.")]
        public string TargetPath { get; set; }

        [Option('o', "outdir",
            MetaValue = "$(TargetDir)",
            HelpText = "The output directory. In most cases, should be $(TargetDir) in MSBuild Macros.")]
        public string TargetDir { get; set; }

        [Option('v', "version",
            Required = false,
            Default = "static",
            MetaValue = "static|assembly",
            HelpText = "The target version. [static] will take the version from the ModInfoAttribute. [assembly] will use the version of the assembly itself.")]
        public string VersionType { get; set; }
    }
}