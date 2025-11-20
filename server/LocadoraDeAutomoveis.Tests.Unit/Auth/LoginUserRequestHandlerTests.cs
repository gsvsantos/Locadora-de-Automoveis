using LocadoraDeAutomoveis.Application.Auth.Commands.Login;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth;

[TestClass]
public sealed class LoginUserRequestHandlerTests
{
    private LoginUserRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";
    private const string _password = "@CleberDS123!";

    private Mock<SignInManager<User>> signInManagerMock = null!;
    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<ITokenProvider> tokenProviderMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<ILogger<LoginUserRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.signInManagerMock = new Mock<SignInManager<User>>(
            this.userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            null!, null!, null!, null!
        );

        this.tokenProviderMock = new Mock<ITokenProvider>();
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.loggerMock = new Mock<ILogger<LoginUserRequestHandler>>();

        this.handler = new LoginUserRequestHandler(
            this.signInManagerMock.Object,
            this.userManagerMock.Object,
            this.tokenProviderMock.Object,
            this.unitOfWorkMock.Object,
            this.loggerMock.Object
        );
    }

    #region Login Tests (Happy Path)
    [TestMethod]
    public async Task Handler_ShouldLogin_And_GenerateAccessToken_Successfully()
    {
        // Arrange
        LoginUserRequest request = new(_userName, _password);

        User user = new()
        {
            UserName = request.UserName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber
        };

        this.userManagerMock
            .Setup(u => u.FindByNameAsync(user.UserName))
            .ReturnsAsync(user);

        this.signInManagerMock
            .Setup(p => p.PasswordSignInAsync(
                user.UserName!, request.Password,
                false, true))
            .ReturnsAsync(SignInResult.Success);

        UserAuthenticatedDto authenticatedUser = new()
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        TokenResponse accessToken = new()
        {
            Key = "accessToken-simulation",
            Expiration = DateTime.UtcNow.AddMinutes(30),
            User = authenticatedUser
        };

        this.tokenProviderMock
            .Setup(t => t.GenerateAccessToken(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == user.FullName &&
                    usr.Email == user.Email &&
                    usr.PhoneNumber == user.PhoneNumber
                    )
                ))
            .ReturnsAsync(accessToken);

        // Act
        Result<TokenResponse> result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        this.userManagerMock.Verify(u => u.FindByNameAsync(request.UserName), Times.Once);

        this.signInManagerMock.Verify(p =>
            p.PasswordSignInAsync(
                user.UserName, request.Password,
                false, true
            ), Times.Once
        );

        this.tokenProviderMock.Verify(t =>
            t.GenerateAccessToken(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName && usr.FullName == user.FullName &&
                    usr.Email == user.Email && usr.PhoneNumber == user.PhoneNumber
                )
            ), Times.Once
        );

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(accessToken.Key, result.Value.Key);
        Assert.AreEqual(accessToken.Expiration, result.Value.Expiration);
        Assert.AreEqual(accessToken.User.Id, result.Value.User.Id);
        Assert.AreEqual(accessToken.User.FullName, result.Value.User.FullName);
        Assert.AreEqual(accessToken.User.Email, result.Value.User.Email);
    }
    #endregion
}
