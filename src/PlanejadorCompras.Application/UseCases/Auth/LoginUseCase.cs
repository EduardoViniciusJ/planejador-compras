using PlanejadorCompras.Application.Features.Authentication.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResultDto> ExecuteAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || user.PasswordHash is null)
        {
            // Keep the expensive password work on failed lookups to reduce timing-based enumeration.
            _passwordHasher.HashPassword(request.Password);
            throw new UnauthorizedException("E-mail ou senha inválidos.", "invalid_credentials");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("E-mail ou senha inválidos.", "invalid_credentials");
        }

        if (!user.IsEmailConfirmed)
        {
            throw new ForbiddenException(
                "Por favor, confirme seu e-mail antes de fazer login.",
                "email_not_confirmed");
        }

        // We don't have user.Name in User entity, so we pass string.Empty or email as name
        var tokenResult = _jwtTokenService.GenerateAccessToken(
            new GenerateAccessTokenRequestDto(user.Id, user.Email, user.Email));

        return new LoginResultDto(
            tokenResult.AccessToken,
            tokenResult.ExpiresAtUtc);
    }
}
