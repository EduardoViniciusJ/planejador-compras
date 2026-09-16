using PlanejadorCompras.Application.Features.Authentication.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class ResetPasswordUseCase
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordUseCase(
        IUserTokenRepository userTokenRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userTokenRepository = userTokenRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var userToken = await _userTokenRepository.GetByTokenAsync(request.Token, "PasswordReset", cancellationToken);

        if (userToken is null || !userToken.IsValid())
        {
            throw new BadRequestException("Token inválido ou expirado.", "invalid_or_expired_token");
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        userToken.User.UpdatePassword(newPasswordHash);
        userToken.User.ConfirmEmail();
        
        await _userTokenRepository.RemoveByUserAndTypeAsync(
            userToken.UserId,
            "PasswordReset",
            cancellationToken);
        _userRepository.Update(userToken.User);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
