using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("Impersonation Actor Dto Domain - Unit Tests")]
public sealed class ImpersonationActorDtoTests
{
    [TestMethod]
    public void IssuedRefreshTokenDtoConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Guid actorUserId = Guid.NewGuid();
        Guid actorTenantId = Guid.NewGuid();
        string actorUserName = "ImpersonatedUser";
        string actorEmail = "impersonatedUser@dasilva.teste";

        ImpersonationActorDto dto = new(
            actorUserId,
            actorTenantId,
            actorUserName,
            actorEmail
        );

        // Assert
        Assert.AreEqual(actorUserId, dto.ActorUserId);
        Assert.AreEqual(actorTenantId, dto.ActorTenantId);
        Assert.AreEqual(actorUserName, dto.ActorUserName);
        Assert.AreEqual(actorEmail, dto.ActorEmail);
    }
}
