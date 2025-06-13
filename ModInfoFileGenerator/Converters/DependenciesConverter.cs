namespace ModInfoFileGenerator.Converters;

/// <summary>
///     Ripped straight from the game, this converter helps to serialise and deserialise mod dependencies.
/// </summary>
/// <seealso cref="JsonConverter" />
public class DependenciesConverter : JsonConverter
{
    /// <summary>
    ///     Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>
    /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IEnumerable<ModDependency>).IsAssignableFrom(objectType);
    }

    /// <summary>
    ///     Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>
    /// The object value.
    /// </returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        return JObject
            .Load(reader)
            .Properties()
            .Select(prop => new ModDependency(prop.Name, (string)prop.Value))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    ///     Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var modDependency in (IEnumerable<ModDependency>)value)
        {
            writer.WritePropertyName(modDependency.ModID);
            writer.WriteValue(modDependency.Version);
        }

        writer.WriteEndObject();
    }
}