namespace ItNewsIntelligenceHub.Application.Common.Exceptions;

public sealed class ExternalServiceException : Exception
{
    public ExternalServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}