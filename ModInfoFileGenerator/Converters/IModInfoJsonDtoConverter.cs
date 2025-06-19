namespace ModInfoFileGenerator.Converters;

/// <summary>
///     Provides an interface for converting a <see cref="ModInfoAttribute"/> from an assembly into a <see cref="ModInfoJsonDto"/>.
///     This is used to abstract the conversion logic for dependency injection and testing.
/// </summary>
public interface IModInfoJsonDtoConverter
{
    /// <summary>
    ///     Populates a DTO object with information from a given assembly and command line options.
    /// </summary>
    /// <param name="assembly">The assembly to extract mod info from.</param>
    /// <param name="options">The parsed command line arguments.</param>
    /// <returns>A populated <see cref="ModInfoJsonDto"/>.</returns>
    ModInfoJsonDto PopulateJsonDto(Assembly assembly, PackagerCommandLineArgs options);
}
