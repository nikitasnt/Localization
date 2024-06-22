using System.Globalization;

namespace Localization.Manager;

/// <summary>
/// String localization manager. Allows you to register string localization sources <see cref="ILocalizationSource"/>
/// and receive localized versions of strings from these sources.
/// </summary>
public class LocalizationManager
{
    public string GetString(string localizationCode, CultureInfo? cultureInfo = null)
    {
        if (cultureInfo == null)
        {
            cultureInfo = CultureInfo.CurrentCulture;
        }

        throw new NotImplementedException();
    }

    public bool TryGetString(string localizationCode, out bool result)
    {
        var cultureInfo = CultureInfo.CurrentCulture;

        throw new NotImplementedException();
    }
    
    public bool TryGetString(string localizationCode, CultureInfo cultureInfo, out string result)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        throw new NotImplementedException();
    }

    public LocalizationManager RegisterSource(ILocalizationSource localizationSource)
    {   
        throw new NotImplementedException();
    }
}