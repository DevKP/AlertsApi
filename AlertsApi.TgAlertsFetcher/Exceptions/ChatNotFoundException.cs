namespace AlertsApi.WTelegram.Hosting.Exceptions;

public class ChatNotFoundException : Exception
{
    public ChatNotFoundException(string message) : base(message)
    {
    }

    public ChatNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ChatNotFoundException()
    {
    }
}