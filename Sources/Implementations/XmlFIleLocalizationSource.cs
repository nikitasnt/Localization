using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Localization.Manager;
using Localization.Manager.Exceptions;
using Localization.Sources.Exceptions;

namespace Localization.Sources.Implementations;

public class XmlFIleLocalizationSource : ILocalizationSource
{
    /// <summary>
    /// The foreign key is the name of the culture information. The internal key is the localization code. The internal
    /// value is a localized string.
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, string>> _repo;

    private XmlFIleLocalizationSource()
    {
        _repo = new Dictionary<string, Dictionary<string, string>>();
    }

    public static async Task<XmlFIleLocalizationSource> CreateAsync(TextReader textReader,
        CancellationToken cancellationToken)
    {
        try
        {
            var xDoc = await XDocument.LoadAsync(textReader, LoadOptions.None, cancellationToken);

            var source = new XmlFIleLocalizationSource();

            // Xml reading and filling repo dictionary.
            foreach (var cultureElem in xDoc.Root!.Elements("culture"))
            {
                var cultureName = cultureElem.Attribute("name")!.Value; // Get culture name from element.

                var dictForCurrentCulture = new Dictionary<string, string>();

                foreach (var localizedStrBuLocalizationCode in cultureElem.Elements())
                {
                    // Element name is a localization code.
                    var localizationCode = localizedStrBuLocalizationCode.Name.LocalName;
                    // Element value is a localized string.
                    var localizedString = localizedStrBuLocalizationCode.Value;

                    dictForCurrentCulture.Add(localizationCode, localizedString);
                }

                source._repo.Add(cultureName, dictForCurrentCulture);
            }

            return source;
        }
        catch (XmlException e)
        {
            throw new WrongXmlFileContentException("The XML file has wrong content.", e);
        }
        catch (NullReferenceException e)
        {
            throw new WrongXmlFileContentException("The XML file has wrong content.", e);
        }
        catch (ArgumentException e)
        {
            throw new WrongXmlFileContentException("The XML file has wrong content.", e);
        }
    }

    public string this[string localizationCode, CultureInfo cultureInfo]
    {
        get
        {
            try
            {
                return _repo[cultureInfo.Name][localizationCode];
            }
            catch (KeyNotFoundException e)
            {
                throw new LocalizedStringNotFoundException(
                    "The XML file did not have the required culture or localization code.", e);
            }
        }
    }

    public bool TryGetLocalizedString(string localizationCode, CultureInfo cultureInfo, out string? result)
    {
        if (_repo.TryGetValue(cultureInfo.Name, out var cultureLocalizations))
        {
            return cultureLocalizations.TryGetValue(localizationCode, out result);
        }

        result = null;
        return false;
    }
}