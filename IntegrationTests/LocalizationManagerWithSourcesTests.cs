using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using Localization.Manager;
using Localization.Manager.Exceptions;
using Localization.Sources.Implementations;

namespace IntegrationTests;

[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public class LocalizationManagerWithSourcesTests
{
    #region Resourse and XML file localization sources

    private static async Task<LocalizationManager> ConstructFor_ResourceAndXmlFileLocalizationSources()
    {
        var resourceSource = new ResourceLocalizationSource(new ResourceManager(
            "IntegrationTests.Resources.Localization", typeof(LocalizationManagerWithSourcesTests).Assembly));
        var xmlFileSource = await XmlFIleLocalizationSource.CreateAsync(new StreamReader(@"..\..\..\Localization.xml"),
            CancellationToken.None);

        return new LocalizationManager()
            .RegisterSource(resourceSource)
            .RegisterSource(xmlFileSource);
    }

    [Fact]
    public async Task GetString_ShouldThrowException_WhenLocalizationCodeDoesntExistsInAnySource()
    {
        // Arrange.
        var manager = await ConstructFor_ResourceAndXmlFileLocalizationSources();
        
        // Act.
        var act = () => manager.GetString("hwAnother", new CultureInfo("en-US"));

        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }
    
    [Fact]
    public async Task GetString_ShouldThrowException_WhenCultureDoesntExistsInAnySource()
    {
        // Arrange.
        var manager = await ConstructFor_ResourceAndXmlFileLocalizationSources();
        
        // Act.
        var act = () => manager.GetString("hw", new CultureInfo("es-ES"));

        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }
    
    [Fact]
    public async Task GetString_ShouldReturnLocalizedStr_WhenItsExistsInXmlFile()
    {
        // Arrange.
        var manager = await ConstructFor_ResourceAndXmlFileLocalizationSources();
        
        // Act.
        var result = manager.GetString("hw", new CultureInfo("de-DE"));

        // Assert.
        Assert.Equal("Hallo Welt!", result);
    }
    
    [Fact]
    public async Task GetString_ShouldReturnLocalizedStr_WhenItsExistsInResource()
    {
        // Arrange.
        var manager = await ConstructFor_ResourceAndXmlFileLocalizationSources();
        
        // Act.
        var result = manager.GetString("hw", new CultureInfo("ru-RU"));

        // Assert.
        Assert.Equal("Привет, мир из .resx!", result);
    }
    
    [Fact]
    public async Task GetString_ShouldReturnLocalizedStr_FromFirstRegisteredSource_WhenItsExistsInTwoSources()
    {
        // Arrange.
        var manager = await ConstructFor_ResourceAndXmlFileLocalizationSources();
        
        // Act.
        var result = manager.GetString("hw", new CultureInfo("en-US"));

        // Assert.
        Assert.Equal("Hello World from .resx!", result);
        Assert.NotEqual("Hello World!", result);
    }

    #endregion

    #region Two XML file localization sources with different XML files
    
    private static async Task<LocalizationManager> ConstructFor_TwoXmlFileLocalizationSources()
    {
        var xmlFileSource1 = await XmlFIleLocalizationSource.CreateAsync(new StreamReader(@"..\..\..\Localization.xml"),
            CancellationToken.None);
        var xmlFileSource2 =
            await XmlFIleLocalizationSource.CreateAsync(new StreamReader(@"..\..\..\AnotherLocalization.xml"),
                CancellationToken.None);

        return new LocalizationManager()
            .RegisterSource(xmlFileSource1)
            .RegisterSource(xmlFileSource2);
    }

    [Fact]
    public async Task GetString_ShouldReturnLocalizedStr_FromFirstRegisteredXmlFileSource_WhenItsExistsInTwoSources()
    {
        // Arrange.
        var manager = await ConstructFor_TwoXmlFileLocalizationSources();
        
        // Act.
        var result = manager.GetString("hw", new CultureInfo("en-US"));

        // Assert.
        Assert.Equal("Hello World!", result);
        Assert.NotEqual("Hello World! ANOTHER", result);
    }

    #endregion
}