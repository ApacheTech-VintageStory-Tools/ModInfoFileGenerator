using System.Reflection.Emit;
using Vintagestory.API.Common;

namespace ModInfoFileGenerator.Tests.Unit;

/// <summary>
///     xUnit fixture for creating dynamic assemblies with a ModInfoAttribute for testing.
/// </summary>
public class AssemblyBuilderFixture
{
    /// <summary>
    ///     Creates a dynamic assembly with a ModInfoAttribute applied at the assembly level.
    /// </summary>
    /// <param name="name">The mod name.</param>
    /// <param name="modId">The mod ID.</param>
    /// <param name="description">The mod description.</param>
    /// <param name="side">The mod side (e.g., "Universal").</param>
    /// <param name="version">The mod version.</param>
    /// <param name="requiredOnClient">Is required on client.</param>
    /// <param name="requiredOnServer">Is required on server.</param>
    /// <param name="networkVersion">Network version.</param>
    /// <param name="website">Website URL.</param>
    /// <param name="contributors">Contributors array.</param>
    /// <param name="authors">Authors array.</param>
    /// <returns>A dynamic <see cref="Assembly"/> with the ModInfoAttribute applied.</returns>
    public Assembly CreateAssemblyWithModInfoAttribute(
        string name = "Test Mod",
        string modId = "testmod",
        string description = "Test Description",
        string side = "Universal",
        string version = "1.0.0",
        bool requiredOnClient = true,
        bool requiredOnServer = true,
        string networkVersion = "1.0.0",
        string website = "https://example.com",
        string[]? contributors = null,
        string[]? authors = null
    )
    {
        var assemblyName = new AssemblyName($"TestModAssembly_{Guid.NewGuid()}");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var modInfoType = typeof(ModInfoAttribute);
        var modInfoCtor = modInfoType.GetConstructor([
            typeof(string), // name
            typeof(string)  // modId
        ]) ?? throw new InvalidOperationException("Could not find ModInfoAttribute constructor.");

        // Get PropertyInfos for named properties
        var namedProperties = new[]
        {
            modInfoType.GetProperty("Description")!,
            modInfoType.GetProperty("Side")!,
            modInfoType.GetProperty("Version")!,
            modInfoType.GetProperty("RequiredOnClient")!,
            modInfoType.GetProperty("RequiredOnServer")!,
            modInfoType.GetProperty("NetworkVersion")!,
            modInfoType.GetProperty("Website")!,
            modInfoType.GetProperty("Contributors")!,
            modInfoType.GetProperty("Authors")!
        };
        var propertyValues = new object[]
        {
            description,
            side,
            version,
            requiredOnClient,
            requiredOnServer,
            networkVersion,
            website,
            contributors ?? ["Test Contributor"],
            authors ?? ["Test Author"]
        };

        var attrBuilder = new CustomAttributeBuilder(
            modInfoCtor,
            [name, modId],
            namedProperties,
            propertyValues
        );

        assemblyBuilder.SetCustomAttribute(attrBuilder);
        moduleBuilder.DefineType("DummyType", TypeAttributes.Public);
        return assemblyBuilder;
    }
}
