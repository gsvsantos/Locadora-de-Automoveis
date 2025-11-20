using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Employees;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees;

[TestClass]
public class GetAllEmployeeRequestHandlerTests
{
    private GetAllEmployeeRequestHandler handler;

    private Mock<IRepositoryEmployee> repositoryMock = null!;
    private Mock<ILogger<GetAllEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryMock = new Mock<IRepositoryEmployee>();
        this.loggerMock = new Mock<ILogger<GetAllEmployeeRequestHandler>>();

        this.handler = new GetAllEmployeeRequestHandler(
            this.repositoryMock.Object,
            this.loggerMock.Object);
    }

    #region GetAllEmployees Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllEmployees_Successfully()
    {
        // Arrange
        List<Employee> employees =
        [
            new Employee("Employee 1", DateTimeOffset.Now.AddYears(-2), 50000m),
            new Employee("Employee 2", DateTimeOffset.Now.AddYears(-1), 60000m)
        ];

        this.repositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(employees);

        GetAllEmployeeRequest request = new();

        // Act
        Result<GetAllEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        IEnumerable<GetAllEmployeeDto> employeesCreated = result.Value.Employees;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.Value.Quantity);
        Assert.AreEqual(employees.Count, employeesCreated.Count());
        Assert.IsTrue(employeesCreated.Any(e => e.FullName.Equals("Employee 1")));
        Assert.IsTrue(employeesCreated.Any(e => e.FullName.Equals("Employee 2")));
    }
    #endregion
}
