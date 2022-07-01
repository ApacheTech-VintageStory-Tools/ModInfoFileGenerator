using System.Collections.Generic;
using ModInfoFileGenerator.Converters;
using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace ModInfoFileGenerator.Model
{
    [JsonObject]
    public record ModInfoJsonDto
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; init; }

        [JsonProperty("name")] public string Name { get; init; }

        [JsonProperty("modId", Required = Required.Always)]
        public string ModId { get; init; }

        [JsonProperty("side", Required = Required.Always)]
        public string Side { get; init; }

        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; init; }

        [JsonProperty("version", Required = Required.Always)]
        public string Version { get; init; } = "0.1.0";

        [JsonProperty("website", NullValueHandling = NullValueHandling.Ignore)]
        public string Website { get; init; }

        [JsonProperty("authors", Required = Required.Always)]
        public IReadOnlyList<string> Authors { get; init; }

        [JsonProperty("contributors", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string> Contributors { get; init; }

        [JsonProperty("requiredOnClient", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RequiredOnClient { get; init; }

        [JsonProperty("requiredOnServer", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RequiredOnServer { get; init; }

        [JsonProperty("networkVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string NetworkVersion { get; init; }

        [JsonProperty("worldConfig", NullValueHandling = NullValueHandling.Ignore)]
        public string WorldConfig { get; init; }

        [JsonProperty("dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(DependenciesConverter))]
        public IReadOnlyList<ModDependency> Dependencies { get; init; }
    }
}