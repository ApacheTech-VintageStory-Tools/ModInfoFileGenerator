namespace ModInfoFileGenerator.Model;

/// <summary>
///    Represents the ModInfo for the mod.
/// </summary>
[JsonObject]
public record ModInfoJsonDto
{
    /// <summary>
    ///     The type of the mod (e.g., "mod", "coremod").
    /// </summary>
    [JsonProperty("type", Required = Required.Always)]
    public string Type { get; init; }

    /// <summary>
    ///     The display name of the mod.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; init; }

    /// <summary>
    ///     The unique identifier for the mod.
    /// </summary>
    [JsonProperty("modId", Required = Required.Always)]
    public string ModId { get; init; }

    /// <summary>
    ///     The side(s) the mod runs on (e.g., "Client", "Server", "Universal").
    /// </summary>
    [JsonProperty("side", Required = Required.Always)]
    public string Side { get; init; }

    /// <summary>
    ///     A description of the mod's functionality or purpose.
    /// </summary>
    [JsonProperty("description", Required = Required.Always)]
    public string Description { get; init; }

    /// <summary>
    ///     The version of the mod, defaulting to "0.1.0" if not specified.
    /// </summary>
    [JsonProperty("version", Required = Required.Always)]
    public string Version { get; init; } = "0.1.0";

    /// <summary>
    ///     The website URL for the mod or its documentation.
    /// </summary>
    [JsonProperty("website", NullValueHandling = NullValueHandling.Ignore)]
    public string Website { get; init; }

    /// <summary>
    ///     The list of primary authors of the mod.
    /// </summary>
    [JsonProperty("authors", Required = Required.Always)]
    public IReadOnlyList<string> Authors { get; init; }

    /// <summary>
    ///     The list of contributors to the mod.
    /// </summary>
    [JsonProperty("contributors", NullValueHandling = NullValueHandling.Ignore)]
    public IReadOnlyList<string> Contributors { get; init; }

    /// <summary>
    ///     Indicates if the mod is required on the client side.
    /// </summary>
    [JsonProperty("requiredOnClient", NullValueHandling = NullValueHandling.Ignore)]
    public bool? RequiredOnClient { get; init; }

    /// <summary>
    ///     Indicates if the mod is required on the server side.
    /// </summary>
    [JsonProperty("requiredOnServer", NullValueHandling = NullValueHandling.Ignore)]
    public bool? RequiredOnServer { get; init; }

    /// <summary>
    ///     The network version required for compatibility.
    /// </summary>
    [JsonProperty("networkVersion", NullValueHandling = NullValueHandling.Ignore)]
    public string NetworkVersion { get; init; }

    /// <summary>
    ///     The world configuration file or settings for the mod.
    /// </summary>
    [JsonProperty("worldConfig", NullValueHandling = NullValueHandling.Ignore)]
    public string WorldConfig { get; init; }

    /// <summary>
    ///     The list of dependencies required by the mod.
    /// </summary>
    [JsonProperty("dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [JsonConverter(typeof(DependenciesConverter))]
    public IReadOnlyList<ModDependency> Dependencies { get; init; }
}