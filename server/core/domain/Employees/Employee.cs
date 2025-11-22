using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Employees;

public class Employee : BaseEntity<Employee>
{
    public string FullName { get; set; } = string.Empty;
    public DateTimeOffset AdmissionDate { get; set; }
    public decimal Salary { get; set; }

    public Employee() { }
    public Employee(string fullName, DateTimeOffset admissionDate, decimal salary) : this()
    {
        this.FullName = fullName;
        this.AdmissionDate = admissionDate;
        this.Salary = salary;
    }

    public void AssociateUser(User user)
    {
        this.User = user;
        this.UserId = user.Id;
    }

    public void AssociateTenant(Guid tenantId) => this.TenantId = tenantId;

    public void Deactivate() => this.IsActive = false;

    public override void Update(Employee updatedEmployee)
    {
        this.FullName = updatedEmployee.FullName;
        this.AdmissionDate = updatedEmployee.AdmissionDate;
        this.Salary = updatedEmployee.Salary;
    }
}
