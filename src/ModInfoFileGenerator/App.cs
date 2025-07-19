namespace ModInfoFileGenerator;

/// <summary>
///     The main application entry point for the mod info file generator.
///     Handles command line parsing and coordinates the conversion process.
/// </summary>
/// <remarks>
///     Initialises a new instance of the <see cref="App"/> class.
/// </remarks>
/// <param name="modInfoJsonDtoConverter">The converter service for mod info DTOs.</param>
/// <param name="logger">The logger for application events.</param>
public class App
{
    /// <summary>
    ///     Runs the application with the specified command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    internal static async Task RunAsync(string[] args)
    {
        Log.Information("Starting mod info file generation process.");
        await Parser.Default
            .ParseArguments<PackagerCommandLineArgs>(args)
            .WithParsedAsync(ConvertAsync);
        Log.Information("Finished processing command line arguments.");
    }

    /// <summary>
    ///     Converts the specified assembly to a modinfo.json file using the provided options.
    /// </summary>
    /// <param name="option">The parsed command line arguments.</param>
    public static async Task ConvertAsync(PackagerCommandLineArgs option)
    {
        Log.Information("Beginning conversion for target assembly: {TargetPath}", option.TargetPath);
        if (!Path.IsPathRooted(option.TargetPath))
            option.TargetPath = Path.Combine(Environment.CurrentDirectory, option.TargetPath);

        var assemblyFile = new FileInfo(option.TargetPath);

        if (!assemblyFile.Exists)
        {
            Log.Error("No file was found at the given location: {TargetPath}", option.TargetPath);
            throw new FileNotFoundException("No file was found at the given location.", option.TargetPath);
        }

        if (!assemblyFile.Extension.Equals(".dll"))
        {
            Log.Error("The selected file is not a .dll file: {TargetPath}", option.TargetPath);
            throw new DllNotFoundException("The selected file is not a .dll file.");
        }

        var outDir = option.TargetDir ?? assemblyFile.DirectoryName!;
        var assembly = Assembly.LoadFile(assemblyFile.FullName);
        Log.Information("Loaded assembly from {AssemblyPath}", assemblyFile.FullName);
        var modInfo = new ModInfoJsonDtoConverter().PopulateJsonDto(assembly, option);
        var json = JsonConvert.SerializeObject(modInfo, Formatting.Indented);
        await File.WriteAllTextAsync(Path.Combine(outDir, "modinfo.json"), json);

        Log.Information("Process complete: modinfo.json file has been created at {OutputPath}", Path.Combine(outDir, "modinfo.json"));
    }
}