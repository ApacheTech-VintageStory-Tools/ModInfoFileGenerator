namespace ModInfoFileGenerator;

/// <summary>
///     Represents the command line arguments for the mod info file generator packager.
///     Used to specify the target assembly, output directory, and versioning mode.
/// </summary>
public sealed class PackagerCommandLineArgs
{
    /// <summary>
    ///     The path to the target assembly. In most cases, this should be $(TargetPath) in MSBuild Macros.
    /// </summary>
    [Option('a', "assembly",
        Required = true,
        MetaValue = "$(TargetPath)",
        HelpText = "The target assembly. In most cases, should be $(TargetPath) in MSBuild Macros.")]
    public string TargetPath { get; set; }

    /// <summary>
    ///     The output directory for the generated modinfo.json file. In most cases, this should be $(TargetDir) in MSBuild Macros.
    /// </summary>
    [Option('o', "outdir",
        MetaValue = "$(TargetDir)",
        HelpText = "The output directory. In most cases, should be $(TargetDir) in MSBuild Macros.")]
    public string TargetDir { get; set; }

    /// <summary>
    ///     The versioning mode. [static] will take the version from the ModInfoAttribute. [assembly] will use the version of the assembly itself.
    /// </summary>
    [Option('v', "version",
        Required = false,
        Default = "static",
        MetaValue = "static|assembly",
        HelpText = "The target version. [static] will take the version from the ModInfoAttribute. [assembly] will use the version of the assembly itself.")]
    public string VersionType { get; set; }
}