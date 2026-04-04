namespace PlanejadorCompras.Domain.Entities;

public sealed class User
{
    private User(Guid id, string googleId, string email, DateTime createdAt)
    {
        Id = id;
        GoogleId = googleId;
        Email = email;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string GoogleId { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static User Create(string googleId, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(googleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(email); 

        return new User(
            Guid.NewGuid(),
            googleId.Trim(),
            email.Trim().ToLowerInvariant(),
            DateTime.UtcNow);
    }
}
