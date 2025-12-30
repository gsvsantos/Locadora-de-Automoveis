using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("ITenantProvider Domain - Unit Tests")]
public sealed class IUserContextTests
{
    public class StubUserContext : IUserContext
    {
        public Guid? UserId { get; set; }

        public bool IsInRole(string roleName)
        {
            return true;
        }
    }

    [TestMethod]
    public void GetUserId_WhenUserIdIsNull_ShouldReturnEmptyGuid()
    {
        // Arrange
        StubUserContext context = new()
        {
            UserId = null
        };

        // Act
        Guid result = ((IUserContext)context).GetUserId();

        // Assert
        Assert.AreEqual(Guid.Empty, result);
    }

    [TestMethod]
    public void GetUserId_WhenUserIdHasValue_ShouldReturnValue()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        StubUserContext context = new()
        {
            UserId = expectedId
        };

        // Act
        Guid result = ((IUserContext)context).GetUserId();

        // Assert
        Assert.AreEqual(expectedId, result);
    }
}
