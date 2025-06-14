namespace ModInfoFileGenerator.Model;

/// <summary>
///    Represents the ModInfo for the mod.
/// </summary>
[JsonObject]
public record ModInfoJsonDto
{
    /// <summary>
    ///     The URL of the JSON schema to follow.
    /// </summary>
    [JsonProperty("$schema", Required = Required.Always)]
    public required string Schema { get; init; }

    /// <summary>
    ///     The type of the mod (e.g., "Code", "Content", or "Theme").
    /// </summary>
    [JsonProperty("type", Required = Required.Always)]
    public required string Type { get; init; }

    /// <summary>
    ///     The display name of the mod.
    /// </summary>
    [JsonProperty("name", Required = Required.Always)]
    public required string Name { get; init; }

    /// <summary>
    ///     The unique identifier for the mod.
    /// </summary>
    [JsonProperty("modId", Required = Required.Always)]
    public required string ModId { get; init; }

    /// <summary>
    ///     The version of the mod, defaulting to "0.1.0" if not specified.
    /// </summary>
    [JsonProperty("version", Required = Required.Always)]
    public required string Version { get; init; } = "0.1.0";

    /// <summary>
    ///     A description of the mod's functionality or purpose.
    /// </summary>
    [JsonProperty("description", Required = Required.Always)]
    public required string Description { get; init; }

    /// <summary>
    ///     The list of primary authors of the mod.
    /// </summary>
    [JsonProperty("authors", Required = Required.Always)]
    public required IReadOnlyList<string> Authors { get; init; }

    /// <summary>
    ///     The list of contributors to the mod.
    /// </summary>
    [JsonProperty("contributors", Required = Required.Always)]
    public required IReadOnlyList<string> Contributors { get; init; }

    /// <summary>
    ///     The website URL for the mod or its documentation.
    /// </summary>
    [JsonProperty("website", Required = Required.Always)]
    public required string Website { get; init; }

    /// <summary>
    ///     The side(s) the mod runs on (e.g., "Client", "Server", "Universal").
    /// </summary>
    [JsonProperty("side", Required = Required.Always)]
    public required string Side { get; init; }

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
    ///     The list of dependencies required by the mod.
    /// </summary>
    [JsonProperty("dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [JsonConverter(typeof(DependenciesConverter))]
    public IReadOnlyList<ModDependency> Dependencies { get; init; }
}