using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class GoogleLoginUseCase
{
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IUserRepository _userRepository;

    public GoogleLoginUseCase(
        IGoogleTokenValidator googleTokenValidator,
        IUserRepository userRepository)
    {
        _googleTokenValidator = googleTokenValidator;
        _userRepository = userRepository;
    }

    public async Task<GoogleLoginResponseDto> ExecuteAsync(
        GoogleLoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.IdToken);

        var googleUser = await _googleTokenValidator.ValidateAsync(request.IdToken, cancellationToken);
        var user = await _userRepository.GetByGoogleIdAsync(googleUser.GoogleId, cancellationToken);

        if (user is null)
        {
            user = User.Create(googleUser.GoogleId, googleUser.Email);
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }

        return new GoogleLoginResponseDto(user.Id, user.Email, googleUser.Name);
    }
}
