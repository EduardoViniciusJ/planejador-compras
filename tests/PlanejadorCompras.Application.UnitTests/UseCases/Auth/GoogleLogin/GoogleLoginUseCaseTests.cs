using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Auth;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Auth.GoogleLogin;

public sealed class GoogleLoginUseCaseTests
{
    private readonly Mock<IGoogleTokenValidator> _googleTokenValidatorMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task ExecuteAsync_ShouldCreateUser_WhenUserDoesNotExist()
    {
        var request = new GoogleLoginRequestDto("valid-id-token");
        var googleUser = new GoogleUserInfo("google-123", "user@test.com", "Test User");
        User? createdUser = null;

        _googleTokenValidatorMock
            .Setup(x => x.ValidateAsync(request.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUser);

        _userRepositoryMock
            .Setup(x => x.GetByGoogleIdAsync(googleUser.GoogleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => createdUser = user)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _jwtTokenServiceMock
            .Setup(x => x.GenerateAccessToken(It.IsAny<GenerateAccessTokenRequestDto>()))
            .Returns(new GenerateAccessTokenResponseDto("jwt-token", DateTime.UtcNow.AddHours(1)));

        var useCase = CreateUseCase();

        var response = await useCase.ExecuteAsync(request);

        Assert.NotNull(createdUser);
        Assert.Equal("jwt-token", response.AccessToken);
        Assert.True(response.ExpiresAtUtc > DateTime.UtcNow);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _jwtTokenServiceMock.Verify(
            x => x.GenerateAccessToken(It.Is<GenerateAccessTokenRequestDto>(dto =>
                dto.UserId == createdUser.Id &&
                dto.Email == createdUser.Email &&
                dto.Name == googleUser.Name)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReuseExistingUser_WhenUserAlreadyExists()
    {
        var request = new GoogleLoginRequestDto("valid-id-token");
        var googleUser = new GoogleUserInfo("google-123", "existing@test.com", "Existing User");
        var existingUser = User.Create(googleUser.GoogleId, googleUser.Email);

        _googleTokenValidatorMock
            .Setup(x => x.ValidateAsync(request.IdToken, It.IsAny<CancellationToken>()))
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
        Assert.True(response.ExpiresAtUtc > DateTime.UtcNow);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _jwtTokenServiceMock.Verify(
            x => x.GenerateAccessToken(It.Is<GenerateAccessTokenRequestDto>(dto =>
                dto.UserId == existingUser.Id &&
                dto.Email == existingUser.Email &&
                dto.Name == googleUser.Name)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldValidateGoogleTokenUsingRequestIdToken()
    {
        var request = new GoogleLoginRequestDto("valid-id-token");
        var googleUser = new GoogleUserInfo("google-123", "user@test.com", "Test User");
        var existingUser = User.Create(googleUser.GoogleId, googleUser.Email);

        _googleTokenValidatorMock
            .Setup(x => x.ValidateAsync(request.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUser);

        _userRepositoryMock
            .Setup(x => x.GetByGoogleIdAsync(googleUser.GoogleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _jwtTokenServiceMock
            .Setup(x => x.GenerateAccessToken(It.IsAny<GenerateAccessTokenRequestDto>()))
            .Returns(new GenerateAccessTokenResponseDto("jwt-token", DateTime.UtcNow.AddHours(1)));

        var useCase = CreateUseCase();

        await useCase.ExecuteAsync(request);

        _googleTokenValidatorMock.Verify(
            x => x.ValidateAsync(request.IdToken, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentException_WhenIdTokenIsEmpty()
    {
        var request = new GoogleLoginRequestDto(string.Empty);
        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }


    private GoogleLoginUseCase CreateUseCase()
    {
        return new GoogleLoginUseCase(
            _googleTokenValidatorMock.Object,
            _jwtTokenServiceMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }
}
