namespace PlanejadorCompras.Application.Services.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    string Name { get; }
}
