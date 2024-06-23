using System.Resources;

namespace Localization.Manager.Exceptions;

public class LocalizedStringNotFoundException : Exception
{
    public LocalizedStringNotFoundException(string message) : base(message)
    {
    }
    
    public LocalizedStringNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}