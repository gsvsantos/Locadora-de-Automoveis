using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Drivers.Application;

[TestClass]
[TestCategory("Driver Application - Unit Tests")]
public sealed class GetByIdDriverRequestHandlerTests : UnitTestBase
{
    private GetByIdDriverRequestHandler handler = null!;

    private Mock<IRepositoryDriver> repositoryDriverMock = null!;
    private Mock<ILogger<GetByIdDriverRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryDriverMock = new Mock<IRepositoryDriver>();
        this.loggerMock = new Mock<ILogger<GetByIdDriverRequestHandler>>();

        this.handler = new GetByIdDriverRequestHandler(
            this.mapper,
            this.repositoryDriverMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetDriverById (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetDriverById_Successfully()
    {
        // Arrange
        Guid driverId = Guid.NewGuid();
        GetByIdDriverRequest request = new(driverId);
        Client client = Builder<Client>.CreateNew().Build();

        Driver driver = new(
            "Cliente Novo",
            "clienteNovo@email.com",
            "(51) 99999-9999",
            "222.222.222-22",
            "12345",
            DateTimeOffset.Now
        )
        { Id = driverId };
        driver.AssociateClient(client);

        this.repositoryDriverMock
            .Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(driver);

        // Act
        Result<GetByIdDriverResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        DriverDto dto = result.Value.Driver;

        // Assert
        this.repositoryDriverMock
            .Verify(r => r.GetByIdAsync(request.Id), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(driver.FullName, dto.FullName);
        Assert.AreEqual(driver.Email, dto.Email);
        Assert.AreEqual(driver.PhoneNumber, dto.PhoneNumber);
        Assert.AreEqual(driver.Document, dto.Document);
        Assert.AreEqual(driver.LicenseNumber, dto.LicenseNumber);
    }
    #endregion
}
