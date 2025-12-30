using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("IssuedRefreshTokenDto Domain - Unit Tests")]
public sealed class IssuedRefreshTokenDtoTests
{
    [TestMethod]
    public void IssuedRefreshTokenDtoConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        string token = "abc-123-token-secreto";
        DateTimeOffset expiration = DateTimeOffset.UtcNow.AddDays(7);

        IssuedRefreshTokenDto dto = new(token, expiration);

        // Assert
        Assert.AreEqual(token, dto.PlainToken);
        Assert.AreEqual(expiration, dto.ExpirationDateUtc);
    }
}
