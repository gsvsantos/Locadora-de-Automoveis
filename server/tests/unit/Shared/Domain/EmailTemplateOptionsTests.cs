using LocadoraDeAutomoveis.Domain.Shared.Email;

namespace LocadoraDeAutomoveis.Tests.Unit.Shared.Domain;

[TestClass]
[TestCategory("EmailTemplateOptions Domain - Unit Tests")]
public sealed class EmailTemplateOptionsTests
{
    [TestMethod]
    public void AppUrlsOptionsConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        HashSet<string> rawPlaceholderKeys = new(StringComparer.Ordinal);
        EmailTemplateOptions options = new();

        // Assert
        Assert.AreEqual("Templates", options.TemplatesFolderName);
        Assert.AreEqual("pt-BR", options.DefaultLanguage);
        Assert.IsTrue(options.EnableNeutralLanguageFallback);
        Assert.IsTrue(options.HtmlEncodeValues);
        Assert.AreEqual(rawPlaceholderKeys, options.RawPlaceholderKeys);
    }
}
