using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("RefreshToken Domain - Unit Tests")]
public sealed class RefreshTokenTests
{
    [TestMethod]
    public void RefreshTokenConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        DateTimeOffset expirationDate = DateTimeOffset.UtcNow.AddDays(5);
        User userAuthenticated = Builder<User>.CreateNew().Build();

        RefreshToken token = new() { ExpirationDateUtc = expirationDate };
        token.AssociateUser(userAuthenticated);

        // Assert
        Assert.AreEqual(userAuthenticated.Id, token.UserAuthenticatedId);
        Assert.AreEqual(string.Empty, token.TokenHash);
        Assert.IsNotNull(token.CreatedDateUtc.DayOfYear);
        Assert.IsNotNull(token.ExpirationDateUtc);
        Assert.IsNull(token.RevokedDateUtc);
        Assert.IsNull(token.RevocationReason);
        Assert.IsNull(token.ReplacedByTokenHash);
        Assert.IsNull(token.CreationIp);
        Assert.IsNull(token.UserAgent);
        Assert.IsTrue(token.IsValid);
    }

    [TestMethod]
    public void RefreshTokenMethod_Update_ShouldDoNothing_WhenCalled()
    {
        // Arrange
        string originalHash = "hash_original_123";
        string originalIp = "192.168.0.1";
        RefreshToken originalToken = new()
        {
            TokenHash = originalHash,
            CreationIp = originalIp
        };

        RefreshToken changesToken = new()
        {
            TokenHash = "otro_hash",
            CreationIp = "192.168.0.25"
        };

        // Act
        originalToken.Update(changesToken);

        // Assert
        Assert.AreEqual(originalHash, originalToken.TokenHash);
        Assert.AreEqual(originalIp, originalToken.CreationIp);
        Assert.AreNotEqual(changesToken.TokenHash, originalToken.TokenHash);
    }

    [TestMethod]
    public void RefreshTokenProp_IsValid_ShouldReturnTrue_WhenRevokedIsNull_AndIsNotExpired()
    {
        // Arrange & Act
        DateTimeOffset expirationDate = DateTimeOffset.UtcNow.AddDays(5);
        RefreshToken token = new() { ExpirationDateUtc = expirationDate };

        // Assert
        Assert.IsNull(token.RevokedDateUtc);
        Assert.IsTrue(token.IsValid);
    }

    [TestMethod]
    public void RefreshTokenProp_IsValid_ShouldReturnFalse_WhenRevokedHasValue()
    {
        // Arrange & Act
        DateTimeOffset revokedDate = DateTimeOffset.UtcNow;
        RefreshToken token = new() { RevokedDateUtc = revokedDate };

        // Assert
        Assert.IsNotNull(token.RevokedDateUtc);
        Assert.IsFalse(token.IsValid);
    }

    [TestMethod]
    public void RefreshTokenProp_IsValid_ShouldReturnFalse_WhenIsExpired()
    {
        // Arrange & Act
        DateTimeOffset expirationDate = DateTimeOffset.UtcNow.AddDays(-5);
        RefreshToken token = new() { ExpirationDateUtc = expirationDate };

        // Assert
        Assert.IsNull(token.RevokedDateUtc);
        Assert.IsFalse(token.IsValid);
    }
}
