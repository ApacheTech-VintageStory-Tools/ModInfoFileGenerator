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
public class App(IModInfoJsonDtoConverter modInfoJsonDtoConverter, ILogger<App> logger)
{
    private readonly IModInfoJsonDtoConverter _modInfoJsonDtoConverter = modInfoJsonDtoConverter;
    private readonly ILogger<App> _logger = logger;

    /// <summary>
    ///     Creates a new host for the application with the specified command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments passed to the application.</param>
    /// <returns>The configured <see cref="IHost"/> instance ready to run the application.</returns>
    public static IHost CreateHost(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IModInfoJsonDtoConverter, ModInfoJsonDtoConverter>();
                services.AddSingleton<App>();
            })
            .Build();
    }

    /// <summary>
    ///     Runs the application with the specified command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    internal async Task RunAsync(string[] args)
    {
        _logger.LogInformation("Starting mod info file generation process.");
        await Parser.Default
            .ParseArguments<PackagerCommandLineArgs>(args)
            .WithParsedAsync(ConvertAsync);
        _logger.LogInformation("Finished processing command line arguments.");
    }

    /// <summary>
    ///     Converts the specified assembly to a modinfo.json file using the provided options.
    /// </summary>
    /// <param name="option">The parsed command line arguments.</param>
    private async Task ConvertAsync(PackagerCommandLineArgs option)
    {
        _logger.LogInformation("Beginning conversion for target assembly: {TargetPath}", option.TargetPath);
        if (!Path.IsPathRooted(option.TargetPath))
            option.TargetPath = Path.Combine(Environment.CurrentDirectory, option.TargetPath);

        var assemblyFile = new FileInfo(option.TargetPath);

        if (!assemblyFile.Exists)
        {
            _logger.LogError("No file was found at the given location: {TargetPath}", option.TargetPath);
            throw new FileNotFoundException("No file was found at the given location.", option.TargetPath);
        }

        if (!assemblyFile.Extension.Equals(".dll"))
        {
            _logger.LogError("The selected file is not a .dll file: {TargetPath}", option.TargetPath);
            throw new DllNotFoundException("The selected file is not a .dll file.");
        }

        var outDir = option.TargetDir ?? assemblyFile.DirectoryName!;
        var assembly = Assembly.LoadFile(assemblyFile.FullName);
        _logger.LogInformation("Loaded assembly from {AssemblyPath}", assemblyFile.FullName);
        var modInfo = _modInfoJsonDtoConverter.PopulateJsonDto(assembly, option);
        var json = JsonConvert.SerializeObject(modInfo, Formatting.Indented);
        await File.WriteAllTextAsync(Path.Combine(outDir, "modinfo.json"), json);

        _logger.LogInformation("Process complete: modinfo.json file has been created at {OutputPath}", Path.Combine(outDir, "modinfo.json"));
    }
}