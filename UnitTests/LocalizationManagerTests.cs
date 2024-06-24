using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Localization.Manager;
using Localization.Manager.Exceptions;
using Moq;

namespace UnitTests;

[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
public class LocalizationManagerTests
{
    #region Registration

    [Fact]
    public void RegisterSource_ShouldNotAddSameObject() // A fragile test that depends on the private field name.
    {
        // Arrange.
        var source = new Mock<ILocalizationSource>().Object;
        var manager = new LocalizationManager();
        
        // Act.
        manager
            .RegisterSource(source)
            .RegisterSource(source);
        
        // Assert.
        Assert.Single((IEnumerable)typeof(LocalizationManager)
            .GetField("_registeredSources", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(manager)!);
    }

    #endregion
    
    #region Localization manager without registered sources

    [Fact]
    public void GetString_ShouldThrowException_WhenLocalizationSourcesAreMissing()
    {
        // Arrange.
        var manager = new LocalizationManager();
        
        // Act.
        var act = () => manager.GetString(string.Empty);
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }
    
    [Fact]
    public void GetString_WithCultInfo_ShouldThrowException_WhenLocalizationSourcesAreMissing()
    {
        // Arrange.
        var manager = new LocalizationManager();
        
        // Act.
        var act = () => manager.GetString(string.Empty, CultureInfo.InvariantCulture);
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }

    [Fact]
    public void TryGetString_ShouldReturnFalseAndNull_WhenLocalizationSourcesAreMissing()
    {
        // Arrange.
        var manager = new LocalizationManager();
        
        // Act.
        var methodResult = manager.TryGetString(string.Empty, out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    [Fact]
    public void TryGetString_WithCultInfo_ShouldReturnFalseAndNull_WhenLocalizationSourcesAreMissing()
    {
        // Arrange.
        var manager = new LocalizationManager();
        
        // Act.
        var methodResult = manager.TryGetString(string.Empty, CultureInfo.InvariantCulture, out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    #endregion

    /// <summary>
    /// Construct a localization source that will contain the passed localized string, accessible from this source using
    /// the passed localization code and culture information.
    /// </summary>
    /// <param name="localizedStr">A localized string that will be in the source.</param>
    /// <param name="localizationCode">Localization code by which the localized string will be available.</param>
    /// <param name="cultureInfo">Culture information by which the localized string will be available.</param>
    /// <returns>The localization source containing the localized string.</returns>
    private static ILocalizationSource ConstructLocalizationSource(string? localizedStr, string localizationCode,
        CultureInfo cultureInfo)
    {
        var mockLocalizationSource = new Mock<ILocalizationSource>();
        mockLocalizationSource
            .Setup(s => s.TryGetLocalizedString(localizationCode, cultureInfo, out localizedStr))
            .Returns(true);

        return mockLocalizationSource.Object;
    }

    #region Localization manager with single registered source

    [Fact]
    public void GetString_ShouldReturnLocalizedStr_WhenLocalizationSource_ContainsSearchStr()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager =
            new LocalizationManager(ConstructLocalizationSource(expectedString, localizationCode, cultureInfo));
        
        // Act.
        var result = manager.GetString(localizationCode, cultureInfo);
        
        // Assert.
        Assert.Equal(expectedString, result);
    }
    
    [Fact]
    public void TryGetString_ShouldReturnTrueAndLocalizedStr_WhenLocalizationSource_ContainsSearchStr()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager =
            new LocalizationManager(ConstructLocalizationSource(expectedString, localizationCode, cultureInfo));
        
        // Act.
        var methodResult = manager.TryGetString(localizationCode, cultureInfo, out var outResult);
        
        // Assert.
        Assert.True(methodResult);
        Assert.Equal(expectedString, outResult);
    }
    
    [Fact]
    public void GetString_ShouldThrowException_WhenLocalizationSource_DoesntContainsSearchStr_ByCode()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager = new LocalizationManager(ConstructLocalizationSource(expectedString, "anotherCode", cultureInfo));
        
        // Act.
        var act = () => manager.GetString(localizationCode, cultureInfo);
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }
    
    [Fact]
    public void TryGetString_ShouldReturnFalseAndNull_WhenLocalizationSource_DoesntContainsSearchStr_ByCode()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager = new LocalizationManager(ConstructLocalizationSource(expectedString, "anotherCode", cultureInfo));
        
        // Act.
        var methodResult = manager.TryGetString(localizationCode, cultureInfo, out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }
    
    [Fact]
    public void GetString_ShouldThrowException_WhenLocalizationSource_DoesntContainsSearchStr_ByCultInfo()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager =
            new LocalizationManager(ConstructLocalizationSource(expectedString, localizationCode,
                new CultureInfo("de-DE")));
        
        // Act.
        var act = () => manager.GetString(localizationCode, cultureInfo);
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }
    
    [Fact]
    public void TryGetString_ShouldReturnFalseAndNull_WhenLocalizationSource_DoesntContainsSearchStr_ByCultInfo()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager =
            new LocalizationManager(ConstructLocalizationSource(expectedString, localizationCode,
                new CultureInfo("de-DE")));
        
        // Act.
        var methodResult = manager.TryGetString(localizationCode, cultureInfo, out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }
    
    [Fact]
    public void GetString_ShouldThrowException_WhenLocalizationSource_CultInfoDoesntCorrespondCurrentCultInfo()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        var manager =
            new LocalizationManager(ConstructLocalizationSource(expectedString, localizationCode,
                new CultureInfo("de-DE")));
        
        // Act.
        var act = () => manager.GetString(localizationCode);
        
        // Assert.
        Assert.Throws<LocalizedStringNotFoundException>(act);
    }
    
    [Fact]
    public void TryGetString_ShouldReturnFalseAndNull_WhenLocalizationSource_CultInfoDoesntCorrespondCurrentCultInfo()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        var manager =
            new LocalizationManager(ConstructLocalizationSource(expectedString, localizationCode,
                new CultureInfo("de-DE")));
        
        // Act.
        var methodResult = manager.TryGetString(localizationCode, out var outResult);
        
        // Assert.
        Assert.False(methodResult);
        Assert.Null(outResult);
    }

    #endregion

    #region Localization manager with two registered sources

    [Fact]
    public void GetString_ShouldReturnLocalizedStr_WhenOnlySecondLocalizationSource_ContainsSearchStr()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw_en";
        var cultureInfo = new CultureInfo("en-US");
        var manager = new LocalizationManager()
            .RegisterSource(ConstructLocalizationSource("Hallo Welt!", "hw_de", new CultureInfo("de-DE")))
            .RegisterSource(ConstructLocalizationSource(expectedString, localizationCode, cultureInfo));
        
        // Act.
        var result = manager.GetString(localizationCode, cultureInfo);
        
        // Assert.
        Assert.Equal(expectedString, result);
    }
    
    [Fact]
    public void TryGetString_ShouldReturnTrueAndLocalizedStr_WhenOnlySecondLocalizationSource_ContainsSearchStr()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw_en";
        var cultureInfo = new CultureInfo("en-US");
        var manager = new LocalizationManager()
            .RegisterSource(ConstructLocalizationSource("Hallo Welt!", "hw_de", new CultureInfo("de-DE")))
            .RegisterSource(ConstructLocalizationSource(expectedString, localizationCode, cultureInfo));
        
        // Act.
        var methodResult = manager.TryGetString(localizationCode, cultureInfo, out var outResult);
        
        // Assert.
        Assert.True(methodResult);
        Assert.Equal(expectedString, outResult);
    }
    
    [Fact]
    public void GetString_ShouldReturnLocalizedStr_WhenBothLocalizationSources_ContainsSearchStr()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager = new LocalizationManager()
            .RegisterSource(ConstructLocalizationSource(expectedString, localizationCode, cultureInfo))
            .RegisterSource(ConstructLocalizationSource("Hello another world!", localizationCode, cultureInfo));
        
        // Act.
        var result = manager.GetString(localizationCode, cultureInfo);
        
        // Assert.
        Assert.Equal(expectedString, result);
    }
    
    [Fact]
    public void TryGetString_ShouldReturnTrueAndLocalizedStr_WhenBothLocalizationSources_ContainsSearchStr()
    {
        // Arrange.
        var expectedString = "Hello world!";
        var localizationCode = "hw";
        var cultureInfo = new CultureInfo("en-US");
        var manager = new LocalizationManager()
            .RegisterSource(ConstructLocalizationSource(expectedString, localizationCode, cultureInfo))
            .RegisterSource(ConstructLocalizationSource("Hello another world!", localizationCode, cultureInfo));
        
        // Act.
        var methodResult = manager.TryGetString(localizationCode, cultureInfo, out var outResult);
        
        // Assert.
        Assert.True(methodResult);
        Assert.Equal(expectedString, outResult);
    }

    #endregion
}