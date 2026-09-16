using System.Security.Cryptography;

namespace PlanejadorCompras.Domain.Entities;

public sealed class UserToken
{
    private UserToken(Guid id, Guid userId, string token, string type, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        Type = type;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public string Type { get; private set; } = null!; // e.g. "EmailConfirmation", "PasswordReset"
    public DateTime ExpiresAt { get; private set; }

    public User User { get; private set; } = null!;

    public static UserToken Create(Guid userId, string type, TimeSpan validFor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(validFor, TimeSpan.Zero);

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        return new UserToken(Guid.NewGuid(), userId, token, type, DateTime.UtcNow.Add(validFor));
    }

    public bool IsValid() => DateTime.UtcNow <= ExpiresAt;
}
