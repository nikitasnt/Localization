using System.Globalization;
using System.Resources;
using Localization.Manager;
using Localization.Manager.Exceptions;

namespace Localization.Sources.Implementations;

/// <summary>
/// Localization source based on data from resource files.
/// </summary>
/// <param name="resourceManager">Resource manager, with specified resource files.</param>
public class ResourceLocalizationSource(ResourceManager resourceManager) : ILocalizationSource
{
    public string this[string localizationCode, CultureInfo cultureInfo]
    {
        get
        {
            try
            {
                var result = resourceManager.GetString(localizationCode, cultureInfo);

                if (result == null)
                {
                    throw new LocalizedStringNotFoundException("The localized string was not found in the source.");
                }

                return result;
            }
            catch (MissingManifestResourceException e)
            {
                throw new LocalizedStringNotFoundException(
                    $"Culture {cultureInfo.Name} does not have the required resource file.", e);
            }
        }
    }

    public bool TryGetLocalizedString(string localizationCode, CultureInfo cultureInfo, out string? result)
    {
        try
        {
            result = resourceManager.GetString(localizationCode, cultureInfo);
            return result != null;
        }
        catch (MissingManifestResourceException)
        {
            result = null;
            return false;
        }
    }
}