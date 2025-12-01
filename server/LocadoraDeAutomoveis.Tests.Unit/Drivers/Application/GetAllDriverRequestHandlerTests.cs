using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Drivers;

namespace LocadoraDeAutomoveis.Tests.Unit.Drivers.Application;

[TestClass]
[TestCategory("Driver Application - Unit Tests")]
public sealed class GetAllDriverRequestHandlerTests
{
    private GetAllDriverRequestHandler handler = null!;

    private Mock<IRepositoryDriver> repositoryDriverMock = null!;
    private Mock<ILogger<GetAllDriverRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryDriverMock = new Mock<IRepositoryDriver>();
        this.loggerMock = new Mock<ILogger<GetAllDriverRequestHandler>>();

        this.handler = new GetAllDriverRequestHandler(
            this.repositoryDriverMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllDrivers (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllDrivers_Successfully()
    {
        // Arrange
        List<Driver> drivers = Builder<Driver>.CreateListOfSize(10).All()
            .Do(d => d.Client = new())
            .Build().ToList();

        this.repositoryDriverMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(drivers);

        GetAllDriverRequest request = new(null);

        // Act
        Result<GetAllDriverResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<DriverDto> vehiclesDto = [.. result.Value.Drivers];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(drivers.Count, vehiclesDto.Count);

        for (int i = 0; i < drivers.Count; i++)
        {
            for (int j = 0; j < vehiclesDto.Count; j++)
            {
                if (drivers[i].Id == vehiclesDto[j].Id)
                {
                    Assert.AreEqual(drivers[i].FullName, drivers[j].FullName);
                    Assert.AreEqual(drivers[i].Email, drivers[j].Email);
                    Assert.AreEqual(drivers[i].PhoneNumber, drivers[j].PhoneNumber);
                    Assert.AreEqual(drivers[i].Document, drivers[j].Document);
                    Assert.AreEqual(drivers[i].LicenseNumber, drivers[j].LicenseNumber);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveDrivers_Successfully()
    {
        // Arrange
        List<Driver> drivers = Builder<Driver>.CreateListOfSize(10).All()
            .Do(d => d.Client = new())
            .Build().ToList();

        this.repositoryDriverMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. drivers.Take(5)]);

        GetAllDriverRequest request = new(5);

        // Act
        Result<GetAllDriverResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<DriverDto> vehiclesDto = [.. result.Value.Drivers];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, drivers.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, vehiclesDto.Count);

        for (int i = 0; i < drivers.Count; i++)
        {
            for (int j = 0; j < vehiclesDto.Count; j++)
            {
                if (drivers[i].Id == vehiclesDto[j].Id)
                {
                    Assert.AreEqual(drivers[i].FullName, drivers[j].FullName);
                    Assert.AreEqual(drivers[i].Email, drivers[j].Email);
                    Assert.AreEqual(drivers[i].PhoneNumber, drivers[j].PhoneNumber);
                    Assert.AreEqual(drivers[i].Document, drivers[j].Document);
                    Assert.AreEqual(drivers[i].LicenseNumber, drivers[j].LicenseNumber);
                }
            }
        }
    }
    #endregion
}
