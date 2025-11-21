using LocadoraDeAutomoveis.Application.Auth.Commands.Register;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Application;

[TestClass]
[TestCategory("Employee Application - Unit Tests")]
public sealed class RegisterUserRequestHandlerTests
{
    private RegisterUserRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";
    private const string _password = "@CleberDS123!";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<ITokenProvider> tokenProviderMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<ILogger<RegisterUserRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.tokenProviderMock = new Mock<ITokenProvider>();
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.loggerMock = new Mock<ILogger<RegisterUserRequestHandler>>();

        this.handler = new RegisterUserRequestHandler(
            this.userManagerMock.Object,
            this.tokenProviderMock.Object,
            this.unitOfWorkMock.Object,
            this.loggerMock.Object
        );
    }

    #region Register Tests (Happy Path)
    [TestMethod]
    public async Task Handler_ShouldRegisterUser_And_GenerateToken_Successfully()
    {
        // Arrange
        RegisterUserRequest request = new(
            _userName,
            _fullName,
            _email,
            _phoneNumber,
            _password
        );

        User user = new()
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = _phoneNumber
        };

        this.userManagerMock
            .Setup(u => u.CreateAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    ), _password
                ))
            .ReturnsAsync(IdentityResult.Success);

        this.userManagerMock
            .Setup(u => u.AddToRoleAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    ), "Admin"
                ))
            .ReturnsAsync(IdentityResult.Success);

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
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    )
                ))
            .ReturnsAsync(accessToken);

        // Act
        Result<TokenResponse> result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        this.userManagerMock.Verify(u =>
            u.CreateAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName && usr.FullName == request.FullName &&
                    usr.Email == request.Email && usr.PhoneNumber == request.PhoneNumber
                ), _password
            ), Times.Once
        );

        this.tokenProviderMock.Verify(t =>
            t.GenerateAccessToken(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName && usr.FullName == request.FullName &&
                    usr.Email == request.Email && usr.PhoneNumber == request.PhoneNumber
                )
            ), Times.Once
        );

        this.userManagerMock.Verify(u => u.AddToRoleAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName && usr.FullName == request.FullName &&
                    usr.Email == request.Email && usr.PhoneNumber == request.PhoneNumber
                ), "Admin"
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
