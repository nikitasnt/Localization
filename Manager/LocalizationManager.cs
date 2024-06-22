using System.Globalization;
using Localization.Manager.Exceptions;

namespace Localization.Manager;

/// <summary>
/// String localization manager. Allows you to register string localization sources <see cref="ILocalizationSource"/>
/// and receive localized versions of strings from these sources.
/// <remarks>The search for a string in localization sources is carried out in the order in which they were registered.
/// </remarks>
/// </summary>
public sealed class LocalizationManager
{
    /// <summary>
    /// Registered localization sources.
    /// </summary>
    private readonly List<ILocalizationSource> _registeredSources;

    /// <summary>
    /// Manager without registered localization sources.
    /// </summary>
    public LocalizationManager()
    {
        _registeredSources = new List<ILocalizationSource>();
    }

    /// <summary>
    /// The manager will search localization sources starting from
    /// <paramref name="firstLocalizationSource"/>.
    /// </summary>
    /// <param name="firstLocalizationSource">This localization source will be the first search for the localization
    /// string.</param>
    public LocalizationManager(ILocalizationSource firstLocalizationSource)
    {
        _registeredSources = new List<ILocalizationSource> { firstLocalizationSource };
    }
    
    /// <summary>
    /// Register the localization source in the manager.
    /// <remarks>The search for a string in localization sources is carried out in the order in which they were
    /// registered.</remarks>
    /// </summary>
    /// <param name="localizationSource">Localization source.</param>
    /// <returns>Current localization manager.</returns>
    public LocalizationManager RegisterSource(ILocalizationSource localizationSource)
    {
        _registeredSources.Add(localizationSource);
        return this;
    }

    /// <summary>
    /// Get a localized string using the current thread's culture information.
    /// </summary>
    /// <param name="localizationCode">Localization string code.</param>
    /// <returns>Localized string.</returns>
    /// <exception cref="LocalizedStringNotFoundException">The localized string was not found in any source.</exception>
    public string GetString(string localizationCode)
    {
        var cultureInfo = CultureInfo.CurrentCulture;

        return GetString(localizationCode, cultureInfo);
    }

    /// <summary>
    /// Get a localized string.
    /// </summary>
    /// <param name="localizationCode">Localization string code.</param>
    /// <param name="cultureInfo">Culture information to find a localized string.</param>
    /// <returns>Localized string.</returns>
    /// <exception cref="LocalizedStringNotFoundException">The localized string was not found in any source.</exception>
    public string GetString(string localizationCode, CultureInfo cultureInfo)
    {
        foreach (var localizationSource in _registeredSources)
        {
            if (localizationSource.TryGetLocalizedString(localizationCode, cultureInfo, out var result))
            {
                return result!;
            }
        }

        throw new LocalizedStringNotFoundException("The localized string was not found in any source.");
    }

    /// <summary>
    /// Try to get a localized string using the current thread's culture information.
    /// </summary>
    /// <param name="localizationCode">Localization string code.</param>
    /// <param name="result">Localized string.</param>
    /// <returns><see langword="true"/> if the localized string is found in the sources; otherwise
    /// <see langword="false"/>.</returns>
    public bool TryGetString(string localizationCode, out string? result)
    {
        var cultureInfo = CultureInfo.CurrentCulture;

        return TryGetString(localizationCode, cultureInfo, out result);
    }
    
    /// <summary>
    /// Try to get a localized string.
    /// </summary>
    /// <param name="localizationCode">Localization string code.</param>
    /// <param name="cultureInfo">Culture information to find a localized string.</param>
    /// <param name="result">Localized string.</param>
    /// <returns><see langword="true"/> if the localized string is found in the sources; otherwise
    /// <see langword="false"/>.</returns>
    public bool TryGetString(string localizationCode, CultureInfo cultureInfo, out string? result)
    {
        foreach (var localizationSource in _registeredSources)
        {
            if (localizationSource.TryGetLocalizedString(localizationCode, cultureInfo, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }
}