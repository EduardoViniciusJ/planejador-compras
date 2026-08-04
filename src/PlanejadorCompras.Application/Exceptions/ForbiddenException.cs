using System.Net;

namespace PlanejadorCompras.Application.Exceptions;

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message, string errorCode = "forbidden", Exception? innerException = null)
        : base(message, HttpStatusCode.Forbidden, errorCode, innerException)
    {
    }
}
