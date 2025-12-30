using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("RefreshTokenOptions Domain - Unit Tests")]
public sealed class RefreshTokenOptionsTests
{
    [TestMethod]
    public void RefreshTokenOptionsConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        RefreshTokenOptions options = new();

        // Assert
        Assert.AreEqual(7, options.ExpirationInDays);
        Assert.AreEqual(string.Empty, options.PepperSecret);
        Assert.AreEqual("LocadoraDeAutomoveis.RefreshToken", options.CookieName);
    }
}
