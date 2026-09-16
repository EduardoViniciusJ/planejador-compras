using PlanejadorCompras.Application.Features.Authentication.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class ForgotPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IFrontendUrlService _frontendUrlService;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordUseCase(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IEmailService emailService,
        IFrontendUrlService frontendUrlService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _emailService = emailService;
        _frontendUrlService = frontendUrlService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            // Do not reveal that the user does not exist
            return;
        }

        await _userTokenRepository.RemoveByUserAndTypeAsync(
            user.Id,
            "PasswordReset",
            cancellationToken);

        var token = UserToken.Create(user.Id, "PasswordReset", TimeSpan.FromHours(1));
        await _userTokenRepository.AddAsync(token, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var resetLink = _frontendUrlService.BuildPasswordResetUrl(token.Token);
        var emailBody = $"<p>Você solicitou a redefinição de senha. Clique no link para criar uma nova senha: <a href='{resetLink}'>Redefinir Senha</a></p>";
        
        await _emailService.SendEmailAsync(user.Email, "Redefinição de Senha", emailBody, cancellationToken);
    }
}
