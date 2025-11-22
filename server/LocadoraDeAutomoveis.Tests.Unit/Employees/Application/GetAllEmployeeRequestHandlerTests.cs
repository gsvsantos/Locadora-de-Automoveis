using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Employees;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees.Application;

[TestClass]
[TestCategory("Employee Application - Unit Tests")]
public sealed class GetAllEmployeeRequestHandlerTests
{
    private GetAllEmployeeRequestHandler handler;

    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<ILogger<GetAllEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.loggerMock = new Mock<ILogger<GetAllEmployeeRequestHandler>>();

        this.handler = new GetAllEmployeeRequestHandler(
            this.repositoryEmployeeMock.Object,
            this.loggerMock.Object);
    }

    #region GetAllEmployees Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllEmployees_Successfully()
    {
        // Arrange
        List<Employee> employees = Builder<Employee>.CreateListOfSize(10).Build().ToList();

        this.repositoryEmployeeMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(employees);

        GetAllEmployeeRequest request = new(null);

        // Act
        Result<GetAllEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<EmployeeDto> employeesDto = [.. result.Value.Employees];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(employees.Count, employeesDto.Count);

        for (int i = 0; i < employees.Count; i++)
        {
            for (int j = 0; j < employeesDto.Count; j++)
            {
                if (employees[i].Id == employeesDto[j].Id)
                {
                    Assert.AreEqual(employees[i].FullName, employeesDto[j].FullName);
                    Assert.AreEqual(employees[i].AdmissionDate, employeesDto[j].AdmissionDate);
                    Assert.AreEqual(employees[i].Salary, employeesDto[j].Salary);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveEmployees_Successfully()
    {
        // Arrange
        List<Employee> employees = Builder<Employee>.CreateListOfSize(10).Build().ToList();

        this.repositoryEmployeeMock
            .Setup(repo => repo.GetAllAsync(5))
            .ReturnsAsync(employees.Take(5).ToList());

        GetAllEmployeeRequest request = new(5);

        // Act
        Result<GetAllEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<EmployeeDto> employeesDto = [.. result.Value.Employees];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, employees.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, employeesDto.Count);

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < employeesDto.Count; j++)
            {
                if (employees[i].Id == employeesDto[j].Id)
                {
                    Assert.AreEqual(employees[i].FullName, employeesDto[j].FullName);
                    Assert.AreEqual(employees[i].AdmissionDate, employeesDto[j].AdmissionDate);
                    Assert.AreEqual(employees[i].Salary, employeesDto[j].Salary);
                }
            }
        }
    }
    #endregion
}
