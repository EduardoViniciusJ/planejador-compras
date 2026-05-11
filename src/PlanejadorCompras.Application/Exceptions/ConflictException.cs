using System.Net;

namespace PlanejadorCompras.Application.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message, string errorCode = "conflict", Exception? innerException = null)
        : base(message, HttpStatusCode.Conflict, errorCode, innerException)
    {
    }
}
