using System.Net;

namespace PlanejadorCompras.Application.Exceptions;

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message, string errorCode = "bad_request", Exception? innerException = null)
        : base(message, HttpStatusCode.BadRequest, errorCode, innerException)
    {
    }
}
