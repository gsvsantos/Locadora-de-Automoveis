using LocadoraDeAutomoveis.Application.RateServices.Commands.Update;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.RateServices.Application;

[TestClass]
[TestCategory("RateService Application - Unit Tests")]
public sealed class UpdateRateServiceRequestHandlerTests : UnitTestBase
{
    private UpdateRateServiceRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRateService> repositoryRateServiceMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<IValidator<RateService>> validatorMock = null!;
    private Mock<ILogger<UpdateRateServiceRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRateServiceMock = new Mock<IRepositoryRateService>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.validatorMock = new Mock<IValidator<RateService>>();
        this.loggerMock = new Mock<ILogger<UpdateRateServiceRequestHandler>>();

        this.handler = new UpdateRateServiceRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryRateServiceMock.Object,
            this.repositoryRentalMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateBillingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateBillingPlan_Successfully()
    {
        // Arrange
        Guid rateServiceId = Guid.NewGuid();
        RateService rateService = new(
            "GPS",
            20
        )
        { Id = rateServiceId };
        rateService.MarkAsFixed();

        this.repositoryRateServiceMock
            .Setup(r => r.GetByIdAsync(rateServiceId))
            .ReturnsAsync(rateService);

        UpdateRateServiceRequest request = new(
            rateServiceId,
            "Manutencao",
            50,
            false
        );

        RateService updatedRateService = new(
            request.Name,
            request.Price
        );
        updatedRateService.MarkAsDaily();

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<RateService>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryRateServiceMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([rateService]);

        // Act
        Result<UpdateRateServiceResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<RateService>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryRateServiceMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryRateServiceMock
            .Verify(r => r.UpdateAsync(
                rateServiceId,
                It.Is<RateService>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
