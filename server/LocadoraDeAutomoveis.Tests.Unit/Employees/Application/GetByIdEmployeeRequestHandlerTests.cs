using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Employees;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees.Application;

[TestClass]
[TestCategory("Employee Application - Unit Tests")]
public sealed class GetByIdEmployeeRequestHandlerTests
{
    private GetByIdEmployeeRequestHandler handler = null!;

    private const string _fullName = "Employee Teste da Silva";

    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<ILogger<GetByIdEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.loggerMock = new Mock<ILogger<GetByIdEmployeeRequestHandler>>();

        this.handler = new GetByIdEmployeeRequestHandler(
            this.repositoryEmployeeMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetEmployeeById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetEmployeeById_Successfuly()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        GetByIdEmployeeRequest request = new(employeeId);

        Employee employee = new(
            _fullName,
            DateTime.UtcNow,
            3000m
        )
        { Id = employeeId };

        this.repositoryEmployeeMock
            .Setup(repo => repo.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        // Act
        Result<GetByIdEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        EmployeeDto dto = result.Value.Employee;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(employee.Id, dto.Id);
        Assert.AreEqual(employee.FullName, dto.FullName);
        Assert.AreEqual(employee.AdmissionDate, dto.AdmissionDate);
        Assert.AreEqual(employee.Salary, dto.Salary);
    }
    #endregion
}
