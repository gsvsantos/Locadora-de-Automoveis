using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("ITenantProvider Domain - Unit Tests")]
public sealed class ITenantProviderTests
{
    public class StubTenantProvider : ITenantProvider
    {
        public Guid? TenantId { get; set; }
    }

    [TestMethod]
    public void GetTenantId_WhenTenantIdIsNull_ShouldReturnEmptyGuid()
    {
        // Arrange
        StubTenantProvider provider = new()
        {
            TenantId = null
        };

        // Act
        Guid result = ((ITenantProvider)provider).GetTenantId();

        // Assert
        Assert.AreEqual(Guid.Empty, result);
    }

    [TestMethod]
    public void GetTenantId_WhenTenantIdHasValue_ShouldReturnValue()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        StubTenantProvider provider = new()
        {
            TenantId = expectedId
        };

        // Act
        Guid result = ((ITenantProvider)provider).GetTenantId();

        // Assert
        Assert.AreEqual(expectedId, result);
    }
}
