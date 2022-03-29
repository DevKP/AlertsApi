namespace AlertsApi.TgAlertsFramework.Exceptions;

public class UnableToParseMessageException : Exception
{
    public UnableToParseMessageException()
    {
        
    }

    public UnableToParseMessageException(string message) : base(message)
    {
        
    }
}