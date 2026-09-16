using PlanejadorCompras.Application.Features.Authentication.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class ConfirmEmailUseCase
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmEmailUseCase(
        IUserTokenRepository userTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userTokenRepository = userTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ConfirmEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        var userToken = await _userTokenRepository.GetByTokenAsync(request.Token, "EmailConfirmation", cancellationToken);

        if (userToken is null || !userToken.IsValid())
        {
            throw new BadRequestException("Token inválido ou expirado.", "invalid_or_expired_token");
        }

        userToken.User.ConfirmEmail();
        _userTokenRepository.Remove(userToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
