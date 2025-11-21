using LocadoraDeAutomoveis.Application.Employees.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees;

[TestClass]
public class CreateEmployeeRequestHandlerTests
{
    private CreateEmployeeRequestHandler handler = null!;

    private const string _userName = "employeeTesteDS";
    private const string _fullName = "Employee Teste da Silva";
    private const string _email = "employeeTesteDS@gmail.com";
    private const string _phoneNumber = "(51) 90000-0001";
    private const string _password = "@SenhaForte123!";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IValidator<Employee>> validatorMock = null!;
    private Mock<ILogger<CreateEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.validatorMock = new Mock<IValidator<Employee>>();
        this.loggerMock = new Mock<ILogger<CreateEmployeeRequestHandler>>();

        this.handler = new CreateEmployeeRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryEmployeeMock.Object,
            this.tenantProviderMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateEmployee Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateEmployee_Successfully()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        CreateEmployeeRequest request = new(
            _fullName, DateTime.UtcNow, 3000m, _userName,
            _email, _phoneNumber, _password
        );

        User user = new()
        {
            UserName = _userName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber,
            TenantId = tenantId
        };

        this.userManagerMock
            .Setup(u => u.CreateAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    ), _password
                ))
            .ReturnsAsync(IdentityResult.Success);

        this.userManagerMock
            .Setup(u => u.AddToRoleAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    ), "Employee"
                ))
            .ReturnsAsync(IdentityResult.Success);

        Employee employee = new(
            request.FullName,
            request.AdmissionDate,
            request.Salary)
        { User = user, TenantId = tenantId };

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Employee>(emp =>
                    emp.FullName == request.FullName &&
                    emp.AdmissionDate == request.AdmissionDate &&
                    emp.Salary == request.Salary
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryEmployeeMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreateEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Employee>(emp =>
                    emp.FullName == request.FullName &&
                    emp.AdmissionDate == request.AdmissionDate &&
                    emp.Salary == request.Salary
                    ), CancellationToken.None
                ), Times.Once
            );

        this.userManagerMock
            .Verify(u => u.CreateAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    ), _password
                ), Times.Once
            );

        this.userManagerMock
            .Verify(u => u.AddToRoleAsync(
                It.Is<User>(usr =>
                    usr.UserName == request.UserName &&
                    usr.FullName == request.FullName &&
                    usr.Email == request.Email &&
                    usr.PhoneNumber == request.PhoneNumber
                    ), "Employee"
                ), Times.Once
            );

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
