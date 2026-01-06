using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Update;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.RentalExtras.Application;

[TestClass]
[TestCategory("RentalExtra Application - Unit Tests")]
public sealed class UpdateRentalExtraRequestHandlerTests : UnitTestBase
{
    private UpdateRentalExtraRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRentalExtra> repositoryRentalExtraMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<IValidator<RentalExtra>> validatorMock = null!;
    private Mock<ILogger<UpdateRentalExtraRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRentalExtraMock = new Mock<IRepositoryRentalExtra>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.validatorMock = new Mock<IValidator<RentalExtra>>();
        this.loggerMock = new Mock<ILogger<UpdateRentalExtraRequestHandler>>();

        this.handler = new UpdateRentalExtraRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryRentalExtraMock.Object,
            this.repositoryRentalMock.Object,
            this.cacheMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateBillingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateBillingPlan_Successfully()
    {
        // Arrange
        Guid rentalExtraId = Guid.NewGuid();
        RentalExtra rentalExtra = new(
            "GPS",
            20
        )
        { Id = rentalExtraId };
        rentalExtra.MarkAsFixed();

        this.repositoryRentalExtraMock
            .Setup(r => r.GetByIdAsync(rentalExtraId))
            .ReturnsAsync(rentalExtra);

        UpdateRentalExtraRequest request = new(
            rentalExtraId,
            "Manutencao",
            50,
            false,
            0
        );

        RentalExtra updatedRentalExtra = new(
            request.Name,
            request.Price
        );
        updatedRentalExtra.MarkAsDaily();

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<RentalExtra>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryRentalExtraMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([rentalExtra]);

        // Act
        Result<UpdateRentalExtraResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<RentalExtra>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryRentalExtraMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryRentalExtraMock
            .Verify(r => r.UpdateAsync(
                rentalExtraId,
                It.Is<RentalExtra>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
