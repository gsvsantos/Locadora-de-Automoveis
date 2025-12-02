using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth.Domain;

[TestClass]
[TestCategory("User Domain - Unit Tests")]
public sealed class UserTests
{
    [TestMethod]
    public void UserConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        User user = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, user.Id);
        Assert.IsTrue(user.EmailConfirmed);
        Assert.IsTrue(user.PhoneNumberConfirmed);
        Assert.AreEqual(string.Empty, user.FullName);
        Assert.IsNull(user.UserName);
        Assert.IsNull(user.Email);
    }

    [TestMethod]
    public void UserConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        User user = new()
        {
            UserName = "Clebinho",
            FullName = "Clebinho Da Silva",
            Email = "cleber@dasilva.net",
            PhoneNumber = "(99) 99999-9999"
        };

        // Assert
        Assert.AreNotEqual(Guid.Empty, user.Id);
        Assert.IsTrue(user.EmailConfirmed);
        Assert.IsTrue(user.PhoneNumberConfirmed);
        Assert.AreEqual("Clebinho", user.UserName);
        Assert.AreEqual("Clebinho Da Silva", user.FullName);
        Assert.AreEqual("cleber@dasilva.net", user.Email);
        Assert.AreEqual("(99) 99999-9999", user.PhoneNumber);
    }

    [TestMethod]
    public void UserMethod_AssociateTenant_ShouldWorks()
    {
        // Arrange & Act
        Guid tenantId = Guid.NewGuid();

        User user = new();
        user.AssociateTenant(tenantId);

        // Assert
        Assert.AreEqual(tenantId, user.TenantId);
    }
}
