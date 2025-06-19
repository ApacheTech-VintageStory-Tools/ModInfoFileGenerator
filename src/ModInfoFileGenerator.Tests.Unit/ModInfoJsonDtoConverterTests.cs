namespace ModInfoFileGenerator.Tests.Unit;

/// <summary>
///     Unit tests for ModInfoJsonDtoConverter.
/// </summary>
public class ModInfoJsonDtoConverterTests(AssemblyBuilderFixture fixture) 
    : IClassFixture<AssemblyBuilderFixture>
{
    private readonly AssemblyBuilderFixture _fixture = fixture;

    [Fact]
    public void PopulateJsonDto_Throws_WhenNoModInfoAttribute()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ModInfoJsonDtoConverter>>();
        var converter = new ModInfoJsonDtoConverter(logger);
        var assembly = Assembly.GetExecutingAssembly(); // No ModInfoAttribute on this assembly
        var args = new PackagerCommandLineArgs
        {
            TargetPath = "dummy.dll",
            VersionType = "static",
            SchemaUrl = "https://example.com/schema.json"
        };

        // Act & Assert
        Assert.Throws<CustomAttributeFormatException>(() =>
            converter.PopulateJsonDto(assembly, args));
    }

    [Fact]
    public void PopulateJsonDto_ReturnsDto_WhenModInfoAttributePresent()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ModInfoJsonDtoConverter>>();
        var converter = new ModInfoJsonDtoConverter(logger);
        var expectedModId = "testmod";
        var expectedName = "Test Mod";
        var expectedVersion = "1.2.3";
        var expectedSide = "Universal";
        var expectedDescription = "A test mod for unit testing.";
        var expectedRequiredOnClient = true;
        var expectedRequiredOnServer = false;
        var expectedNetworkVersion = "2.0.0";
        var expectedWebsite = "https://unit.test";
        var expectedContributors = new[] { "Alice", "Bob" };
        var expectedAuthors = new[] { "UnitTester" };

        var assembly = _fixture.CreateAssemblyWithModInfoAttribute(
            name: expectedName,
            modId: expectedModId,
            description: expectedDescription,
            side: expectedSide,
            version: expectedVersion,
            requiredOnClient: expectedRequiredOnClient,
            requiredOnServer: expectedRequiredOnServer,
            networkVersion: expectedNetworkVersion,
            website: expectedWebsite,
            contributors: expectedContributors,
            authors: expectedAuthors
        );
        var args = new PackagerCommandLineArgs
        {
            TargetPath = "dummy.dll",
            VersionType = "static",
            SchemaUrl = "https://example.com/schema.json"
        };

        // Act
        var dto = converter.PopulateJsonDto(assembly, args);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(expectedModId, dto.ModId);
        Assert.Equal(expectedName, dto.Name);
        Assert.Equal(expectedVersion, dto.Version);
        Assert.Equal(expectedSide, dto.Side);
        Assert.Equal(expectedDescription, dto.Description);
        Assert.Equal(expectedRequiredOnClient, dto.RequiredOnClient);
        Assert.Equal(expectedRequiredOnServer, dto.RequiredOnServer);
        Assert.Equal(expectedNetworkVersion, dto.NetworkVersion);
        Assert.Equal(expectedWebsite, dto.Website);
        Assert.Equal(expectedContributors, dto.Contributors);
        Assert.Equal(expectedAuthors, dto.Authors);
        Assert.Equal("https://example.com/schema.json", dto.Schema);
    }

    [Fact]
    public void PopulateJsonDto_Throws_WhenModIdIsEmpty()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ModInfoJsonDtoConverter>>();
        var converter = new ModInfoJsonDtoConverter(logger);
        var assembly = _fixture.CreateAssemblyWithModInfoAttribute(
            name: "Test Mod",
            modId: ""
        );
        var args = new PackagerCommandLineArgs
        {
            TargetPath = "dummy.dll",
            VersionType = "static",
            SchemaUrl = "https://example.com/schema.json"
        };

        // Act & Assert
        Assert.ThrowsAny<Exception>(() =>
            converter.PopulateJsonDto(assembly, args));
    }
}