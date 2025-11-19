using LocadoraDeAutomoveis.Core.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Auth;

[TestClass]
public sealed class UserTests
{
    [TestMethod]
    public void User_DefaultConstructor_ShouldInitializeProperties()
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
    public void User_ParameterizedConstructor_ShouldWorks()
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
}
