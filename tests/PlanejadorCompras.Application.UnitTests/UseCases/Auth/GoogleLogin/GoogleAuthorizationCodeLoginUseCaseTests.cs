using PlanejadorCompras.Application.Features.Authentication.Contracts;
using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Auth;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Auth.GoogleLogin;

public sealed class GoogleAuthorizationCodeLoginUseCaseTests
{
    private readonly Mock<IGoogleAuthorizationCodeExchanger> _googleAuthorizationCodeExchangerMock = new();
    private readonly Mock<IGoogleTokenValidator> _googleTokenValidatorMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task ExecuteAsync_ShouldExchangeCodeAndLoginWithReturnedIdToken()
    {
        var request = new GoogleAuthorizationCodeLoginRequestDto("valid-auth-code");
        var idToken = "valid-id-token";
        var googleUser = new GoogleUserInfo("google-123", "user@test.com", "Test User");
        var existingUser = User.Create(googleUser.GoogleId, googleUser.Email);

        _googleAuthorizationCodeExchangerMock
            .Setup(x => x.ExchangeForIdTokenAsync(request.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(idToken);

        _googleTokenValidatorMock
            .Setup(x => x.ValidateAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUser);

        _userRepositoryMock
            .Setup(x => x.GetByGoogleIdAsync(googleUser.GoogleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _jwtTokenServiceMock
            .Setup(x => x.GenerateAccessToken(It.IsAny<GenerateAccessTokenRequestDto>()))
            .Returns(new GenerateAccessTokenResponseDto("jwt-token", DateTime.UtcNow.AddHours(1)));

        var useCase = CreateUseCase();

        var response = await useCase.ExecuteAsync(request);

        Assert.Equal("jwt-token", response.AccessToken);

        _googleAuthorizationCodeExchangerMock.Verify(
            x => x.ExchangeForIdTokenAsync(request.Code, It.IsAny<CancellationToken>()),
            Times.Once);
        _googleTokenValidatorMock.Verify(
            x => x.ValidateAsync(idToken, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentException_WhenCodeIsEmpty()
    {
        var request = new GoogleAuthorizationCodeLoginRequestDto(string.Empty);
        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    private GoogleAuthorizationCodeLoginUseCase CreateUseCase()
    {
        var googleLoginUseCase = new GoogleLoginUseCase(
            _googleTokenValidatorMock.Object,
            _jwtTokenServiceMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);

        return new GoogleAuthorizationCodeLoginUseCase(
            _googleAuthorizationCodeExchangerMock.Object,
            googleLoginUseCase);
    }
}
