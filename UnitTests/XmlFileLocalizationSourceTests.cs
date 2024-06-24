using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Localization.Manager.Exceptions;
using Localization.Sources.Exceptions;
using Localization.Sources.Implementations;

namespace UnitTests;

[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public class XmlFileLocalizationSourceTests
{
    private static StringReader GenerateStringReader()
    {
        var xmlContent = """
                         <?xml version="1.0" encoding="UTF-8"?>
                         <localization>
                            <culture name="en-US">
                               <hw>Hello World!</hw>
                            </culture>
                         </localization>
                         """;

        return new StringReader(xmlContent);
    }

    #region XML file localization source creating

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenXmlContent_IsEmpty()
    {
        // Arrange.
        var badXmlContent = string.Empty;
        var textReader = new StringReader(badXmlContent);
        
        // Act.
        var act = () => XmlFIleLocalizationSource.CreateAsync(textReader, CancellationToken.None);
        
        // Assert.
        await Assert.ThrowsAsync<WrongXmlFileContentException>(act);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenXmlContent_WithCultureElemWithoutNameAttr()
    {
        // Arrange.
        var badXmlContent = """
                            <localization>
                               <culture>
                                  <hw>Hello World!</hw>
                               </culture>
                            </localization>
                            """;
        var textReader = new StringReader(badXmlContent);
        
        // Act.
        var act = () => XmlFIleLocalizationSource.CreateAsync(textReader, CancellationToken.None);
        
        // Assert.
        await Assert.ThrowsAsync<WrongXmlFileContentException>(act);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenXmlContent_TwoLocalizationCodesInCulture()
    {
        // Arrange.
        var badXmlContent = """
                            <localization>
                               <culture name="en-US">
                                  <hw>Hello World!</hw>
                                  <hw>Hello another World!</hw>
                               </culture>
                            </localization>
                            """;
        var textReader = new StringReader(badXmlContent);
        
        // Act.
        var act = () => XmlFIleLocalizationSource.CreateAsync(textReader, CancellationToken.None);
        
        // Assert.
        await Assert.ThrowsAsync<WrongXmlFileContentException>(act);
    }

    #endregion
    
    #region Indexer

    [Fact]
    public async Task Indexer_ShouldThrowException_WhenResourceForCultureDoesntExists()
    {
        // Arrange.
        var source = await XmlFIleLocalizationSource.CreateAsync(GenerateStringReader(), CancellationToken.None);
        
        // Act.
        var act = () => source["hw", new CultureInfo("de-DE")];
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }

    [Fact]
    public async Task Indexer_ShouldThrowException_WhenLocalizationCodeDoesntExists()
    {
        // Arrange.
        var source = await XmlFIleLocalizationSource.CreateAsync(GenerateStringReader(), CancellationToken.None);
        
        // Act.
        var act = () => source["hwAnother", new CultureInfo("en-US")];
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }

    [Fact]
    public async Task Indexer_ShouldReturnLocalizedStr_WhenResourceWithCodeExistsForCulture()
    {
        // Arrange.
        var source = await XmlFIleLocalizationSource.CreateAsync(GenerateStringReader(), CancellationToken.None);
        
        // Act.
        var result = source["hw", new CultureInfo("en-US")];
        
        // Assert.
        Assert.Equal("Hello World!", result);
    }

    #endregion

    #region Try get localized string method

    [Fact]
    public async Task TryGetLocalizedString_ShouldReturnFalseAndNull_WhenResourceForCultureDoesntExists()
    {
        // Arrange.
        var source = await XmlFIleLocalizationSource.CreateAsync(GenerateStringReader(), CancellationToken.None);
        
        // Act.
        var methodResult = source.TryGetLocalizedString("hw", new CultureInfo("de-DE"), out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    [Fact]
    public async Task TryGetLocalizedString_ShouldReturnFalseAndNull_WhenLocalizationCodeDoesntExists()
    {
        // Arrange.
        var source = await XmlFIleLocalizationSource.CreateAsync(GenerateStringReader(), CancellationToken.None);
        
        // Act.
        var methodResult = source.TryGetLocalizedString("hwAnother", new CultureInfo("en-US"), out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    [Fact]
    public async Task TryGetLocalizedString_ShouldReturnTrueAndLocalizedStr_WhenResourceWithCodeExistsForCulture()
    {
        // Arrange.
        var source = await XmlFIleLocalizationSource.CreateAsync(GenerateStringReader(), CancellationToken.None);
        
        // Act.
        var methodResult = source.TryGetLocalizedString("hw", new CultureInfo("en-US"), out var outResult);
        
        // Assert.
        Assert.True(methodResult);
        Assert.Equal("Hello World!", outResult);
    }

    #endregion
}