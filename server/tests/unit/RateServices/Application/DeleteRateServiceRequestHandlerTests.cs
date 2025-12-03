using LocadoraDeAutomoveis.Application.RateServices.Commands.Delete;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.RateServices.Application;

[TestClass]
[TestCategory("RateService Application - Unit Tests")]
public sealed class DeleteRateServiceRequestHandlerTests
{
    private DeleteRateServiceRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRateService> repositoryRateServiceMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteRateServiceRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRateServiceMock = new Mock<IRepositoryRateService>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteRateServiceRequestHandler>>();

        this.handler = new DeleteRateServiceRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryRateServiceMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteRateService Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteRateService_Successfully()
    {
        // Arrange
        Guid rateServiceId = Guid.NewGuid();
        DeleteRateServiceRequest request = new(
            rateServiceId
        );

        RateService rateService = new(
            "GPS",
            20
        )
        { Id = rateServiceId };
        rateService.MarkAsFixed();

        this.repositoryRateServiceMock
            .Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(rateService);

        this.repositoryRateServiceMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteRateServiceResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryRateServiceMock
            .Verify(r => r.GetByIdAsync(request.Id),
            Times.Once);

        this.repositoryRateServiceMock
            .Verify(r => r.DeleteAsync(request.Id),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
