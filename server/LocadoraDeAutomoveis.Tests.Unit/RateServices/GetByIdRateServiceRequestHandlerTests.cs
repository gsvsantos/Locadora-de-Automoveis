using LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;
using LocadoraDeAutomoveis.Application.RateServices.Commands.GetById;
using LocadoraDeAutomoveis.Domain.RateServices;

namespace LocadoraDeAutomoveis.Tests.Unit.RateServices;

[TestClass]
[TestCategory("RateService Application - Unit Tests")]
public sealed class GetByIdRateServiceRequestHandlerTests
{
    private GetByIdRateServiceRequestHandler handler = null!;

    private Mock<IRepositoryRateService> repositoryRateServiceMock = null!;
    private Mock<ILogger<GetByIdRateServiceRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryRateServiceMock = new Mock<IRepositoryRateService>();
        this.loggerMock = new Mock<ILogger<GetByIdRateServiceRequestHandler>>();

        this.handler = new GetByIdRateServiceRequestHandler(
            this.repositoryRateServiceMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetRateServiceById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetPricingPlansById_Successfuly()
    {
        // Arrange
        Guid rateServiceId = Guid.NewGuid();
        GetByIdRateServiceRequest request = new(
            rateServiceId
        );

        RateService rateService = new(
            "GPS",
            20
        )
        { Id = rateServiceId };
        rateService.MarkAsFixed();

        this.repositoryRateServiceMock
            .Setup(r => r.GetByIdAsync(rateServiceId))
            .ReturnsAsync(rateService);

        // Act
        Result<GetByIdRateServiceResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        RateServiceDto dto = result.Value.RateService;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(rateService.Id, dto.Id);
        Assert.AreEqual(rateService.Name, dto.Name);
        Assert.AreEqual(rateService.Price, dto.Price);
        Assert.AreEqual(rateService.IsFixed, dto.IsFixed);
    }
    #endregion
}
