using LocadoraDeAutomoveis.Application.Employees.Commands.SelfUpdate;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.Employees.Application;

[TestClass]
[TestCategory("Employee Application - Unit Tests")]
public sealed class SelfUpdateEmployeeRequestHandlerTests : UnitTestBase
{
    private SelfUpdateEmployeeRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<IValidator<Employee>> validatorMock = null!;
    private Mock<ILogger<SelfUpdateEmployeeRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.validatorMock = new Mock<IValidator<Employee>>();
        this.loggerMock = new Mock<ILogger<SelfUpdateEmployeeRequestHandler>>();

        this.handler = new SelfUpdateEmployeeRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryEmployeeMock.Object,
            this.cacheMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region SelfUpdateEmployee Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldSelfUpdateEmployee_Successfully()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        SelfUpdateEmployeeRequest request = new(
            userId,
            "Clebinho Da Silva SelfUpdate"
        );

        User user = new()
        {
            Id = userId,
            UserName = _userName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber,
        };

        Employee employee = new(
            _fullName,
            DateTime.UtcNow,
            3000m
        );
        employee.AssociateUser(user);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(request.Id.ToString()))
            .ReturnsAsync(user);

        this.repositoryEmployeeMock
            .Setup(r => r.GetByUserIdAsync(request.Id))
            .ReturnsAsync(employee);

        this.repositoryEmployeeMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([employee]);

        Employee updatedEmployee = new(
            request.FullName,
            employee.AdmissionDate,
            employee.Salary
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
            .Setup(r => r.UpdateAsync(
                employee.Id,
                It.Is<Employee>(emp =>
                    emp.FullName == updatedEmployee.FullName &&
                    emp.AdmissionDate == updatedEmployee.AdmissionDate &&
                    emp.Salary == updatedEmployee.Salary
                    )
                ))
            .Verifiable();

        Assert.IsNotNull(employee.User);

        this.userManagerMock
            .Setup(um => um.UpdateAsync(employee.User))
            .ReturnsAsync(IdentityResult.Success);

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync());

        // Act
        Result<SelfUpdateEmployeeResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.userManagerMock
            .Verify(um => um.FindByIdAsync(request.Id.ToString()),
            Times.Once);

        this.repositoryEmployeeMock
            .Verify(r => r.GetByUserIdAsync(request.Id),
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
                employee.Id,
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
