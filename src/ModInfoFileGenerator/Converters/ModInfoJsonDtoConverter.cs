namespace ModInfoFileGenerator.Converters;

/// <summary>
///     Converts <see cref="ModInfoAttribute"/> from a given assembly, to a <see cref="ModInfoJsonDto"/>, ready to be processed.
///     This service is used for extracting and converting mod metadata for output as JSON.
/// </summary>
/// <remarks>
///     Initialises a new instance of the <see cref="ModInfoJsonDtoConverter"/> class.
/// </remarks>
/// <param name="logger">The logger to use for trace output.</param>
public class ModInfoJsonDtoConverter(ILogger<ModInfoJsonDtoConverter> logger) : IModInfoJsonDtoConverter
{
    private readonly ILogger<ModInfoJsonDtoConverter> _logger = logger;

    /// <summary>
    ///     Populates a DTO object with information from a given assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="options">The parsed args passed in by the user.</param>
    /// <returns>A populated <see cref="ModInfoJsonDto"/>.</returns>
    /// <exception cref="CustomAttributeFormatException">No ModInfoAttribute found in assembly.</exception>
    public ModInfoJsonDto PopulateJsonDto(Assembly assembly, PackagerCommandLineArgs options)
    {
        _logger.LogInformation("Populating ModInfoJsonDto for assembly: {AssemblyLocation}", assembly.Location);
        var modInfoAttribute = ExtractModFileInfo(assembly)
            ?? throw new CustomAttributeFormatException("No ModInfoAttribute found in assembly.");
        _logger.LogInformation("ModInfoAttribute found: {ModId}", modInfoAttribute.ModID);
        var side = ParseAppSideOrThrow(modInfoAttribute.Side);
        var version = ResolveVersion(modInfoAttribute, assembly, options);
        var dependencies = FindAllModDependencies(assembly);
        _logger.LogInformation("Dependencies found: {DependencyCount}", dependencies.Count);
        return new ModInfoJsonDto
        {
            Schema = options.SchemaUrl,
            Type = "Code",
            Name = modInfoAttribute.Name,
            ModId = modInfoAttribute.ModID,
            Side = modInfoAttribute.Side,
            Description = modInfoAttribute.Description,
            Version = version,
            Authors = modInfoAttribute.Authors,
            Contributors = modInfoAttribute.Contributors,
            NetworkVersion = modInfoAttribute.NetworkVersion,
            RequiredOnClient = modInfoAttribute.RequiredOnClient && side is not EnumAppSide.Server,
            RequiredOnServer = modInfoAttribute.RequiredOnServer && side is not EnumAppSide.Client,
            Website = modInfoAttribute.Website,
            Dependencies = dependencies
        };
    }

    /// <summary>
    ///     Extracts the mod information from a given assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>An instance of <see cref="ModInfoAttribute"/> populated with information from the assembly.</returns>
    private ModInfoAttribute ExtractModFileInfo(Assembly assembly)
    {
        try
        {
            _logger.LogInformation("Extracting ModInfoAttribute from assembly: {AssemblyLocation}", assembly.Location);
            return assembly.GetCustomAttribute<ModInfoAttribute>();
        }
        catch (FileNotFoundException e)
        {
            var fileName = e.FileName!.Split(',')[0];
            var directory = Path.GetDirectoryName(assembly.Location);
            var filePath = Path.Combine(directory!, $"{fileName}.dll");
            _logger.LogInformation("Loading missing dependency: {FilePath}", filePath);
            Assembly.LoadFrom(filePath);
            return ExtractModFileInfo(assembly);
        }
    }

    /// <summary>
    ///     Parses the mod side string to <see cref="EnumAppSide"/> or throws if invalid.
    /// </summary>
    /// <param name="sideString">The side string to parse.</param>
    /// <returns>The parsed <see cref="EnumAppSide"/> value.</returns>
    /// <exception cref="ArgumentException">Thrown if the side string is invalid.</exception>
    private EnumAppSide ParseAppSideOrThrow(string sideString)
    {
        _logger.LogInformation("Parsing mod side: {SideString}", sideString);
        if (!Enum.TryParse(sideString, true, out EnumAppSide side))
        {
            _logger.LogError("Invalid mod side: {SideString}", sideString);
            throw new ArgumentException($"Cannot parse '{sideString}', must be either 'Client', 'Server' or 'Universal'.");
        }
        return side;
    }

    /// <summary>
    ///     Resolves the version string based on options.
    /// </summary>
    /// <param name="modInfoAttribute">The mod info attribute.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="options">The parsed command line arguments.</param>
    /// <returns>The resolved version string.</returns>
    private string ResolveVersion(ModInfoAttribute modInfoAttribute, Assembly assembly, PackagerCommandLineArgs options)
    {
        _logger.LogInformation("Resolving version for mod: {ModId}, VersionType: {VersionType}", modInfoAttribute.ModID, options.VersionType);
        return options.VersionType == "static"
            ? modInfoAttribute.Version
            : FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion!;
    }

    /// <summary>
    ///     Gathers a list of all mods that this mod depends upon.
    /// </summary>
    /// <param name="assembly">The mod assembly to scan.</param>
    /// <returns>A <see cref="List{ModDependency}"/> populated with any dependent mods, for the specified assembly.</returns>
    private List<ModDependency> FindAllModDependencies(Assembly assembly)
    {
        var dependencies = assembly
            .GetCustomAttributes<ModDependencyAttribute>()
            .Select(p => new ModDependency(p.ModID, p.Version))
            .ToList();
        _logger.LogInformation("Found {Count} mod dependencies.", dependencies.Count);
        return dependencies;
    }
}