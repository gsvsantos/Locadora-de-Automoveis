using LocadoraDeAutomoveis.Core.Domain.Auth;
using LocadoraDeAutomoveis.Core.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Employees;

public class Employee : BaseEntity<Employee>
{
    public string FullName { get; set; } = string.Empty;
    public DateTimeOffset AdmissionDate { get; set; }
    public decimal Salary { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = new User();

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

    public override void Update(Employee registroEditado) => throw new NotImplementedException();
}
