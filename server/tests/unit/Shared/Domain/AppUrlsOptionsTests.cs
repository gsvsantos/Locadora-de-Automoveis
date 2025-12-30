using LocadoraDeAutomoveis.Domain.Shared.Email;

namespace LocadoraDeAutomoveis.Tests.Unit.Shared.Domain;

[TestClass]
[TestCategory("AppUrlsOptions Domain - Unit Tests")]
public sealed class AppUrlsOptionsTests
{
    [TestMethod]
    public void AppUrlsOptionsConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        AppUrlsOptions options = new();

        // Assert
        Assert.AreEqual("", options.AdminApp);
        Assert.AreEqual("", options.PortalApp);
    }
}
