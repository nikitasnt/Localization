using System.Globalization;

namespace Localization.Manager;

/// <summary>
/// Source of localization strings. Allows you to get a localized version of a string from some storage.
/// </summary>
public interface ILocalizationSource
{
    /// <summary>
    /// Get localized string by localization code and culture information.
    /// </summary>
    /// <param name="localizationCode">Localization code.</param>
    /// <param name="cultureInfo">Culture information.</param>
    public string this[string localizationCode, CultureInfo cultureInfo] { get; }

    /// <summary>
    /// Try to get localized string by localization code and culture information
    /// </summary>
    /// <param name="localizationCode">Localization code.</param>
    /// <param name="cultureInfo">Culture information.</param>
    /// <param name="result">Localized string.</param>
    /// <returns><see langword="true"/> if the value was in storage; otherwise <see langword="false"/>.</returns>
    public bool TryGetLocalizedString(string localizationCode, CultureInfo cultureInfo, out string result);
}