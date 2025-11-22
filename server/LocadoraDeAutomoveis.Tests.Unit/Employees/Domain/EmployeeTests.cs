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
    public void EmployeeMethod_Update_ShouldWorks()
    {
        // Arrange
        DateTimeOffset admissionDate = DateTimeOffset.UtcNow;
        Employee employee = new(
            "Employee Teste da Silva",
            admissionDate,
            2469m
            );
        Employee updatedEmployee = new(
            "Employee Updated da Silva",
            admissionDate.AddDays(10),
            3000m
            );

        // Act
        employee.Update(updatedEmployee);

        // Assert
        Assert.AreEqual("Employee Updated da Silva", employee.FullName);
        Assert.AreEqual(admissionDate.AddDays(10), employee.AdmissionDate);
        Assert.AreEqual(3000m, employee.Salary);
    }
}
