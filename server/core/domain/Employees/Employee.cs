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

    public override void Update(Employee updatedEmployee)
    {
        this.FullName = updatedEmployee.FullName;
        this.AdmissionDate = updatedEmployee.AdmissionDate;
        this.Salary = updatedEmployee.Salary;
    }
}
