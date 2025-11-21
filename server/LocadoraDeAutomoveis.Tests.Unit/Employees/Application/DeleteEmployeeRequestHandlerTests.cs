using LocadoraDeAutomoveis.Application.Employees.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees.Application;

[TestClass]
[TestCategory("Employee Application - Unit Tests")]
public sealed class DeleteEmployeeRequestHandlerTests
{
    private DeleteEmployeeRequestHandler handler = null!;

    private const string _userName = "employeeTesteDS";
    private const string _fullName = "Employee Teste da Silva";
    private const string _email = "employeeTesteDS@gmail.com";
    private const string _phoneNumber = "(51) 90000-0001";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<ILogger<DeleteEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.loggerMock = new Mock<ILogger<DeleteEmployeeRequestHandler>>();

        this.handler = new DeleteEmployeeRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryEmployeeMock.Object,
            this.loggerMock.Object
        );
    }
    #region DeleteEmployee Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteEmployee_Successfully()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        DeleteEmployeeRequest request = new(employeeId);

        Employee employee = new(
            _fullName,
            DateTime.UtcNow,
            3000m
        )
        { Id = employeeId };
        employee.AssociateUser(new User
        {
            UserName = _userName,
            Email = _email,
            PhoneNumber = _phoneNumber
        });

        this.repositoryEmployeeMock
            .Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        this.repositoryEmployeeMock
            .Setup(r => r.DeleteAsync(employeeId));

        this.userManagerMock
            .Setup(r => r.DeleteAsync(employee.User));

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync());

        // Act
        Result<DeleteEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryEmployeeMock
            .Verify(r => r.GetByIdAsync(employeeId),
            Times.Once);

        this.repositoryEmployeeMock
            .Verify(r => r.DeleteAsync(employeeId),
            Times.Once);

        this.userManagerMock
            .Verify(r => r.DeleteAsync(employee.User),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
