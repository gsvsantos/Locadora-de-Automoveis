using LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Employees.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees;

[TestClass]
public class GetByIdEmployeeRequestHandlerTests
{
    private GetByIdEmployeeRequestHandler handler = null!;

    private const string _userName = "employeeTesteDS";
    private const string _fullName = "Employee Teste da Silva";
    private const string _email = "employeeTesteDS@gmail.com";
    private const string _phoneNumber = "(51) 90000-0001";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<IValidator<Employee>> validatorMock = null!;
    private Mock<ILogger<GetByIdEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.validatorMock = new Mock<IValidator<Employee>>();
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
