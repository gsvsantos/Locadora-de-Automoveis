using LocadoraDeAutomoveis.Domain.Shared.Email;

namespace LocadoraDeAutomoveis.Tests.Unit.Shared.Domain;

[TestClass]
[TestCategory("MailSettings Domain - Unit Tests")]
public sealed class MailSettingsTests
{
    [TestMethod]
    public void AppUrlsOptionsConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        MailSettings settings = new();

        // Assert
        Assert.AreEqual(string.Empty, settings.Host);
        Assert.IsNotNull(settings.Port);
        Assert.AreEqual(string.Empty, settings.UserName);
        Assert.AreEqual(string.Empty, settings.Password);
        Assert.AreEqual(string.Empty, settings.SenderName);
        Assert.AreEqual(string.Empty, settings.SenderEmail);
    }
}
