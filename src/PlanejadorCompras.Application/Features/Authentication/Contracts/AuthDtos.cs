using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record RegisterRequestDto
{
    public RegisterRequestDto(string email, string password) =>
        (Email, Password) = (email, password);

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; init; }

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; init; }
}

public sealed record LoginRequestDto
{
    public LoginRequestDto(string email, string password) =>
        (Email, Password) = (email, password);

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; init; }

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; init; }
}

public record LoginResultDto(string AccessToken, DateTime ExpiresAtUtc);

public sealed record ForgotPasswordRequestDto
{
    public ForgotPasswordRequestDto(string email) => Email = email;

    [Required, EmailAddress, StringLength(320)]
    public string Email { get; init; }
}

public sealed record ResetPasswordRequestDto
{
    public ResetPasswordRequestDto(string token, string newPassword) =>
        (Token, NewPassword) = (token, newPassword);

    [Required, StringLength(128)]
    public string Token { get; init; }

    [Required, StringLength(128, MinimumLength = 8)]
    public string NewPassword { get; init; }
}

public sealed record ConfirmEmailRequestDto
{
    public ConfirmEmailRequestDto(string token) => Token = token;

    [Required, StringLength(128)]
    public string Token { get; init; }
}
