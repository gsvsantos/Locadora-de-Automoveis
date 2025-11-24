using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.RateServices;

namespace LocadoraDeAutomoveis.Tests.Unit.RateServices;

[TestClass]
[TestCategory("RateService Application - Unit Tests")]
public sealed class GetAllRateServiceRequestHandlerTests
{
    private GetAllRateServiceRequestHandler handler = null;

    private Mock<IRepositoryRateService> repositoryRateServiceMock = null!;
    private Mock<ILogger<GetAllRateServiceRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryRateServiceMock = new Mock<IRepositoryRateService>();
        this.loggerMock = new Mock<ILogger<GetAllRateServiceRequestHandler>>();

        this.handler = new GetAllRateServiceRequestHandler(
            this.repositoryRateServiceMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllRateServices Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllPricingPlans_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<RateService> rateServices = Builder<RateService>.CreateListOfSize(10).All()
            .Do(v => v.Name = random.NextString(1, 5))
            .Do(v => v.Price = random.Decimal())
            .Build().ToList();

        this.repositoryRateServiceMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(rateServices);

        GetAllRateServiceRequest request = new(null);

        // Act
        Result<GetAllRateServiceResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<RateServiceDto> rateServicesDto = [.. result.Value.RateServices];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(rateServices.Count, rateServicesDto.Count);

        for (int i = 0; i < rateServices.Count; i++)
        {
            for (int j = 0; j < rateServicesDto.Count; j++)
            {
                if (rateServices[i].Id == rateServicesDto[j].Id)
                {
                    Assert.AreEqual(rateServices[i].Name, rateServices[j].Name);
                    Assert.AreEqual(rateServices[i].Price, rateServices[j].Price);
                    Assert.AreEqual(rateServices[i].IsFixed, rateServices[j].IsFixed);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFivePricingPlan_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<RateService> rateServices = Builder<RateService>.CreateListOfSize(10).All()
            .Do(v => v.Name = random.NextString(1, 5))
            .Do(v => v.Price = random.Decimal())
            .Build().ToList();

        this.repositoryRateServiceMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. rateServices.Take(5)]);

        GetAllRateServiceRequest request = new(5);

        // Act
        Result<GetAllRateServiceResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<RateServiceDto> rateServicesDto = [.. result.Value.RateServices];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, rateServices.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, rateServicesDto.Count);

        for (int i = 0; i < rateServices.Count; i++)
        {
            for (int j = 0; j < rateServicesDto.Count; j++)
            {
                if (rateServices[i].Id == rateServicesDto[j].Id)
                {
                    Assert.AreEqual(rateServices[i].Name, rateServices[j].Name);
                    Assert.AreEqual(rateServices[i].Price, rateServices[j].Price);
                    Assert.AreEqual(rateServices[i].IsFixed, rateServices[j].IsFixed);
                }
            }
        }
    }
    #endregion
}
