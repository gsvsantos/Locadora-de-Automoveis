using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Delete;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.RentalExtras.Application;

[TestClass]
[TestCategory("RentalExtra Application - Unit Tests")]
public sealed class DeleteRentalExtraRequestHandlerTests
{
    private DeleteRentalExtraRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRentalExtra> repositoryRentalExtraMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteRentalExtraRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRentalExtraMock = new Mock<IRepositoryRentalExtra>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteRentalExtraRequestHandler>>();

        this.handler = new DeleteRentalExtraRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryRentalExtraMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteRentalExtra Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteRentalExtra_Successfully()
    {
        // Arrange
        Guid rentalExtraId = Guid.NewGuid();
        DeleteRentalExtraRequest request = new(
            rentalExtraId
        );

        RentalExtra rentalExtra = new(
            "GPS",
            20
        )
        { Id = rentalExtraId };
        rentalExtra.MarkAsFixed();

        this.repositoryRentalExtraMock
            .Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(rentalExtra);

        this.repositoryRentalExtraMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteRentalExtraResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryRentalExtraMock
            .Verify(r => r.GetByIdAsync(request.Id),
            Times.Once);

        this.repositoryRentalExtraMock
            .Verify(r => r.DeleteAsync(request.Id),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
