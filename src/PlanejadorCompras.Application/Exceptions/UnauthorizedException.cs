using System.Net;

namespace PlanejadorCompras.Application.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message, string errorCode = "unauthorized", Exception? innerException = null)
        : base(message, HttpStatusCode.Unauthorized, errorCode, innerException)
    {
    }
}
