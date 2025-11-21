using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees.Domain;

[TestClass]
[TestCategory("Employee Domain - Unit Tests")]
public sealed class EmployeeTests
{
    [TestMethod]
    public void EmployeeConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Employee employee = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, employee.Id);
        Assert.IsTrue(employee.IsActive);
        Assert.AreEqual(string.Empty, employee.FullName);
        Assert.AreEqual(DateTimeOffset.MinValue, employee.AdmissionDate);
        Assert.AreEqual(0, employee.Salary);
    }

    [TestMethod]
    public void EmployeeConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DateTimeOffset admissionDate = DateTimeOffset.UtcNow;

        Employee employee = new(
            "Employee Teste da Silva",
            admissionDate,
            2469m
            );

        // Assert
        Assert.AreNotEqual(Guid.Empty, employee.Id);
        Assert.IsTrue(employee.IsActive);
        Assert.AreEqual("Employee Teste da Silva", employee.FullName);
        Assert.AreEqual(admissionDate, employee.AdmissionDate);
        Assert.AreEqual(2469m, employee.Salary);
    }

    [TestMethod]
    public void EmployeeMethod_AssociateUser_ShouldWorks()
    {
        // Arrange & Act
        User user = new();

        Employee employee = new();
        employee.AssociateUser(user);

        // Assert
        Assert.AreEqual(user, employee.User);
        Assert.AreEqual(user.Id, employee.User.Id);
    }

    [TestMethod]
    public void EmployeeMethod_AssociateTenant_ShouldWorks()
    {
        // Arrange & Act
        Guid tenantId = Guid.NewGuid();

        Employee employee = new();
        employee.AssociateTenant(tenantId);

        // Assert
        Assert.AreEqual(tenantId, employee.TenantId);
    }

    [TestMethod]
    public void EmployeeMethod_Deactivate_ShouldWorks()
    {
        // Arrange & Act
        Employee employee = new();
        employee.Deactivate();

        // Assert
        Assert.IsFalse(employee.IsActive);
    }
}
