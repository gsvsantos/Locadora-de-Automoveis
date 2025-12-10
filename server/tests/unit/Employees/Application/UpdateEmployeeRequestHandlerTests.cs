using LocadoraDeAutomoveis.Application.Employees.Commands.Update;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees.Application;

[TestClass]
[TestCategory("Employee Application - Unit Tests")]
public sealed class UpdateEmployeeRequestHandlerTests : UnitTestBase
{
    private UpdateEmployeeRequestHandler handler = null!;

    private const string _userName = "employeeTesteDS";
    private const string _fullName = "Employee Teste da Silva";
    private const string _email = "employeeTesteDS@gmail.com";
    private const string _phoneNumber = "(51) 90000-0001";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<IValidator<Employee>> validatorMock = null!;
    private Mock<ILogger<UpdateEmployeeRequestHandler>> loggerMock = null!;

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
        this.loggerMock = new Mock<ILogger<UpdateEmployeeRequestHandler>>();

        this.handler = new UpdateEmployeeRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryEmployeeMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateEmployee Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateEmployee_Successfully()
    {
        // Arrange
        Guid employeeId = Guid.NewGuid();
        UpdateEmployeeRequest request = new(
            employeeId,
            "Employee Teste da Silva Editado",
            DateTime.UtcNow.AddDays(-1),
            5000m
        );

        User user = new()
        {
            UserName = _userName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber,
        };

        Employee employee = new(
            _fullName,
            DateTime.UtcNow,
            3000m
        )
        { Id = employeeId };
        employee.AssociateUser(user);

        this.repositoryEmployeeMock
            .Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        this.repositoryEmployeeMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([employee]);

        Employee updatedEmployee = new(
            request.FullName,
            request.AdmissionDate,
            request.Salary
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Employee>(emp =>
                    emp.FullName == updatedEmployee.FullName &&
                    emp.AdmissionDate == updatedEmployee.AdmissionDate &&
                    emp.Salary == updatedEmployee.Salary
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryEmployeeMock
            .Setup(r => r.UpdateAsync(request.Id, updatedEmployee))
            .Verifiable();

        Assert.IsNotNull(employee.User);

        this.userManagerMock
            .Setup(um => um.UpdateAsync(employee.User))
            .ReturnsAsync(IdentityResult.Success);

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync());

        // Act
        Result<UpdateEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryEmployeeMock
            .Verify(r => r.GetByIdAsync(employeeId),
            Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Employee>(emp =>
                    emp.FullName == updatedEmployee.FullName &&
                    emp.AdmissionDate == updatedEmployee.AdmissionDate &&
                    emp.Salary == updatedEmployee.Salary
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryEmployeeMock
            .Verify(r => r.GetAllAsync(),
            Times.Once);

        this.repositoryEmployeeMock
            .Verify(r => r.UpdateAsync(
                request.Id,
                It.Is<Employee>(emp =>
                    emp.FullName == updatedEmployee.FullName &&
                    emp.AdmissionDate == updatedEmployee.AdmissionDate &&
                    emp.Salary == updatedEmployee.Salary
                    )
                ), Times.Once
            );

        this.userManagerMock
            .Verify(um => um.UpdateAsync(employee.User),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(employee.Id, result.Value.Id);
    }
    #endregion
}
