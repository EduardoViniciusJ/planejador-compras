using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Auth;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.User;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Auth;

public sealed class GoogleLoginUseCaseTests
{
    private readonly Mock<IGoogleTokenValidator> _googleTokenValidatorMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

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

        _userRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = CreateUseCase();

        var response = await useCase.ExecuteAsync(request);

        Assert.NotNull(createdUser);
        Assert.Equal(createdUser.Id, response.UserId);
        Assert.Equal("user@test.com", response.Email);
        Assert.Equal("Test User", response.Name);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

        var useCase = CreateUseCase();

        var response = await useCase.ExecuteAsync(request);

        Assert.Equal(existingUser.Id, response.UserId);
        Assert.Equal(existingUser.Email, response.Email);
        Assert.Equal("Existing User", response.Name);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
            _userRepositoryMock.Object);
    }
}
