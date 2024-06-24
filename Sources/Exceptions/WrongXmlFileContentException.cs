namespace Localization.Sources.Exceptions;

public class WrongXmlFileContentException : Exception
{
    public WrongXmlFileContentException(string message) : base(message)
    {
    }

    public WrongXmlFileContentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}