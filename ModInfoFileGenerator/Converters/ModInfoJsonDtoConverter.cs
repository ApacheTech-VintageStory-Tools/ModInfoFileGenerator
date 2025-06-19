namespace ModInfoFileGenerator.Converters;

/// <summary>
///     Converts <see cref="ModInfoAttribute"/> from a given assembly, to a <see cref="ModInfoJsonDto"/>, ready to be processed.
/// </summary>
internal static class ModInfoJsonDtoConverter
{
    /// <summary>
    ///     Populates a DTO object with information from a given assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="options">The parsed args passed in by the user.</param>
    /// <returns></returns>
    /// <exception cref="CustomAttributeFormatException">No ModInfoAttribute found in assembly.</exception>
    internal static ModInfoJsonDto PopulateJsonDto(this Assembly assembly, PackagerCommandLineArgs options)
    {
        var modInfoAttribute = ExtractModFileInfo(assembly) 
            ?? throw new CustomAttributeFormatException("No ModInfoAttribute found in assembly."); var side = ParseAppSideOrThrow(modInfoAttribute.Side);
        var version = ResolveVersion(modInfoAttribute, assembly, options);
        var dependencies = assembly.FindAllModDependencies();
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
    private static ModInfoAttribute ExtractModFileInfo(Assembly assembly)
    {
        try
        {
            return assembly.GetCustomAttribute<ModInfoAttribute>();
        }
        catch (FileNotFoundException e)
        {
            var fileName = e.FileName!.Split(',')[0];
            var directory = Path.GetDirectoryName(assembly.Location);
            var filePath = Path.Combine(directory!, $"{fileName}.dll");
            Assembly.LoadFrom(filePath);
            return ExtractModFileInfo(assembly);
        }
    }

    /// <summary>
    ///     Parses the mod side string to EnumAppSide or throws if invalid.
    /// </summary>
    private static EnumAppSide ParseAppSideOrThrow(string sideString)
    {
        if (!Enum.TryParse(sideString, true, out EnumAppSide side))
            throw new ArgumentException($"Cannot parse '{sideString}', must be either 'Client', 'Server' or 'Universal'. Defaulting to 'Universal'.");
        return side;
    }

    /// <summary>
    ///     Resolves the version string based on options.
    /// </summary>
    private static string ResolveVersion(ModInfoAttribute modInfoAttribute, Assembly assembly, PackagerCommandLineArgs options)
    {
        return options.VersionType == "static"
            ? modInfoAttribute.Version
            : FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion!;
    }

    /// <summary>
    ///     Gathers a list of all mods that this mod depends upon.
    /// </summary>
    /// <param name="assembly">The mod assembly to scan.</param>
    /// <returns>A <see cref="List{ModDependency}"/> populated with any dependent mods, for the specified assembly.</returns>
    private static List<ModDependency> FindAllModDependencies(this Assembly assembly) => 
    [
        .. assembly
            .GetCustomAttributes<ModDependencyAttribute>()
            .Select(p => new ModDependency(p.ModID, p.Version))
    ];
}