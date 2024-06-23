using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using Localization.Manager.Exceptions;
using Localization.Sources.Implementations;
using Moq;

namespace UnitTests;

[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public class ResourceLocalizationSourceTests
{
    private static ResourceManager ConstructResourceManagerWithResult(string localizationCode, CultureInfo cultureInfo,
        string? resultOfResourceManagerMethod)
    {
        var mock = new Mock<ResourceManager>();
        mock.Setup(rm => rm.GetString(localizationCode, cultureInfo)).Returns(resultOfResourceManagerMethod);
        return mock.Object;
    }

    private static ResourceManager ConstructResourceManagerWithException(string localizationCode,
        CultureInfo cultureInfo, Exception exceptionFromResourceManagerMethod)
    {
        var mock = new Mock<ResourceManager>();
        mock.Setup(rm => rm.GetString(localizationCode, cultureInfo)).Throws(exceptionFromResourceManagerMethod);
        return mock.Object;
    }
    
    #region Indexer

    [Fact]
    public void Indexer_ShouldThrowException_WhenResourceForCultureDoesntExists()
    {
        // Arrange.
        var localizationCode = "hw";
        var culture = new CultureInfo("en-US");
        var source = new ResourceLocalizationSource(ConstructResourceManagerWithException(localizationCode, culture,
            new MissingManifestResourceException(string.Empty)));
        
        // Act.
        var act = () => source[localizationCode, culture];
        
        // Assert.
        var e = Assert.Throws<LocalizedStringNotFoundException>(act);
        Assert.IsType<MissingManifestResourceException>(e.InnerException);
    }

    [Fact]
    public void Indexer_ShouldThrowException_WhenLocalizationCodeDoesntExists()
    {
        // Arrange.
        var localizationCode = "hw";
        var culture = new CultureInfo("en-US");
        var source =
            new ResourceLocalizationSource(ConstructResourceManagerWithResult(localizationCode, culture, null));
        
        // Act.
        var act = () => source[localizationCode, culture];
        
        // Assert.
        var e = Assert.Throws<LocalizedStringNotFoundException>(act);
        Assert.Null(e.InnerException);
    }

    [Fact]
    public void Indexer_ShouldReturnLocalizedStr_WhenResourceWithCodeExistsForCulture()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var culture = new CultureInfo("en-US");
        var source =
            new ResourceLocalizationSource(
                ConstructResourceManagerWithResult(localizationCode, culture, expectedString));

        // Act.
        var result = source[localizationCode, culture];
        
        // Assert.
        Assert.Equal(expectedString, result);
    }

    #endregion

    #region Try get localized string method

    [Fact]
    public void TryGetLocalizedString_ShouldReturnFalseAndNull_WhenResourceForCultureDoesntExists()
    {
        // Arrange.
        var localizationCode = "hw";
        var culture = new CultureInfo("en-US");
        var source = new ResourceLocalizationSource(ConstructResourceManagerWithException(localizationCode, culture,
            new MissingManifestResourceException(string.Empty)));
        
        // Act.
        var methodResult = source.TryGetLocalizedString(localizationCode, culture, out var outResult);

        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    [Fact]
    public void TryGetLocalizedString_ShouldReturnFalseAndNull_WhenLocalizationCodeDoesntExists()
    {
        // Arrange.
        var localizationCode = "hw";
        var culture = new CultureInfo("en-US");
        var source =
            new ResourceLocalizationSource(ConstructResourceManagerWithResult(localizationCode, culture, null));
        
        // Act.
        var methodResult = source.TryGetLocalizedString(localizationCode, culture, out var outResult);

        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    [Fact]
    public void TryGetLocalizedString_ShouldReturnTrueAndLocalizedStr_WhenResourceWithCodeExistsForCulture()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var culture = new CultureInfo("en-US");
        var source =
            new ResourceLocalizationSource(
                ConstructResourceManagerWithResult(localizationCode, culture, expectedString));

        // Act.
        var methodResult = source.TryGetLocalizedString(localizationCode, culture, out var outResult);

        // Assert.
        Assert.True(methodResult);
        Assert.Equal(expectedString, outResult);
    }

    #endregion
}