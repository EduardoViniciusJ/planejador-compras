using System.Net;

namespace PlanejadorCompras.Application.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message, HttpStatusCode statusCode, string errorCode, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public HttpStatusCode StatusCode { get; }

    public string ErrorCode { get; }
}
