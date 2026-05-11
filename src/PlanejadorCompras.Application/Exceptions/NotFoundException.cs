using System.Net;

namespace PlanejadorCompras.Application.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message, string errorCode = "not_found", Exception? innerException = null)
        : base(message, HttpStatusCode.NotFound, errorCode, innerException)
    {
    }
}
