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
    /// <param name="versionType">Type of the version.</param>
    /// <returns></returns>
    /// <exception cref="CustomAttributeFormatException">No ModInfoAttribute found in assembly.</exception>
    internal static ModInfoJsonDto PopulateJsonDto(this Assembly assembly, string versionType)
    {
        var modInfo = ExtractModFileInfo(assembly);
        if (modInfo == null) throw new CustomAttributeFormatException("No ModInfoAttribute found in assembly.");
        var dependencies = assembly.FindAllModDependencies();
        var version = versionType == "static" 
            ? modInfo.Version 
            : FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion!;
        return modInfo.ToDto(version, dependencies);
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
    ///     Converts a vanilla  <see cref="ModInfoAttribute"/> to a <see cref="ModInfoJsonDto"/>, ready for serialisation.
    /// </summary>
    /// <param name="modInfoAttribute">The mod information from the mod assembly.</param>
    /// <param name="version">The mod version to add to the DTO.</param>
    /// <param name="dependencies">A list of any specified mod dependencies, to add to the DTO.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Cannot parse '{modInfo.Side}', must be either 'Client', 'Server' or 'Universal'. Defaulting to 'Universal'.</exception>
    private static ModInfoJsonDto ToDto(this ModInfoAttribute modInfoAttribute, string version,
        IReadOnlyList<ModDependency> dependencies)
    {
        if (!Enum.TryParse(modInfoAttribute.Side, true, out EnumAppSide side))
            throw new ArgumentException(
                $"Cannot parse '{modInfoAttribute.Side}', must be either 'Client', 'Server' or 'Universal'. Defaulting to 'Universal'.");

        var schemaUrl = "https://mods.vintagestory.at/web/schema/modinfo.v2.rc1.json";

        var dto = new ModInfoJsonDto
        {
            Schema = schemaUrl,
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
        return dto;
    }

    /// <summary>
    ///     Gathers a list of all mods that this mod depends upon.
    /// </summary>
    /// <param name="assembly">The mod assembly to scan.</param>
    /// <returns>A <see cref="List{ModDependency}"/> populated with any dependent mods, for the specified assembly.</returns>
    private static List<ModDependency> FindAllModDependencies(this Assembly assembly)
    {
        return assembly
            .GetCustomAttributes<ModDependencyAttribute>()
            .Select(p => new ModDependency(p.ModID, p.Version))
            .ToList();
    }
}