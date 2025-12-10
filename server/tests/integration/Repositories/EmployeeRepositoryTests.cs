using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("EmployeeRepository Infrastructure - Integration Tests")]
public sealed class EmployeeRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_AddAsync_And_GetByUserIdAsync_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName == "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id == Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Employee employee = Builder<Employee>.CreateNew()
            .With(e => e.FullName == "Employee")
            .With(e => e.Id == Guid.NewGuid()).Persist();
        employee.AssociateUser(userEmployee);
        employee.AssociateTenant(tenant.Id);

        // Act
        await this.employeeRepository.AddAsync(employee);

        await this.dbContext.SaveChangesAsync();

        // Act
        Employee? selectedEmployee = await this.employeeRepository.GetByUserIdAsync(userEmployee.Id);

        // Assert
        Assert.IsNotNull(selectedEmployee);
        Assert.AreEqual(employee, selectedEmployee);
    }
}
