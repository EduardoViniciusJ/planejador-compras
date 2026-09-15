using PlanejadorCompras.Application.Features.Authentication.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class RegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUseCase(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (existingUser is not null)
        {
            if (existingUser.GoogleId is not null && existingUser.PasswordHash is null)
            {
                throw new InvalidOperationException("Este e-mail já está cadastrado via Google. Faça login pelo Google ou utilize a opção 'Esqueci minha senha' para cadastrar uma senha.");
            }
            throw new InvalidOperationException("E-mail já cadastrado.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.CreateManualUser(request.Email, passwordHash);
        
        await _userRepository.AddAsync(user, cancellationToken);

        var token = UserToken.Create(user.Id, "EmailConfirmation", TimeSpan.FromDays(1));
        await _userTokenRepository.AddAsync(token, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        // TODO: Replace with the actual frontend URL from configuration
        var confirmLink = $"https://seusite.com/confirmar-email?token={token.Token}";
        var emailBody = $"<p>Por favor, confirme seu e-mail clicando no link: <a href='{confirmLink}'>Confirmar E-mail</a></p>";
        
        await _emailService.SendEmailAsync(user.Email, "Confirmação de E-mail", emailBody, cancellationToken);
    }
}
