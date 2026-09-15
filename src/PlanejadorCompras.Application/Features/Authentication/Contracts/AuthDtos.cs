namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public record RegisterRequestDto(string Email, string Password);
public record LoginRequestDto(string Email, string Password);
public record LoginResultDto(string AccessToken, DateTime ExpiresAtUtc);
public record ForgotPasswordRequestDto(string Email);
public record ResetPasswordRequestDto(string Token, string NewPassword);
public record ConfirmEmailRequestDto(string Token);
