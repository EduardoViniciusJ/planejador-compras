using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Results;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class GoogleLoginUseCase
{
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GoogleLoginUseCase(
        IGoogleTokenValidator googleTokenValidator,
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _googleTokenValidator = googleTokenValidator;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GoogleLoginResultDto> ExecuteAsync(
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
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        var tokenResult = _jwtTokenService.GenerateAccessToken(
            new GenerateAccessTokenRequestDto(user.Id, user.Email, googleUser.Name));

        return new GoogleLoginResultDto(
            tokenResult.AccessToken,
            tokenResult.ExpiresAtUtc);
    }
}
