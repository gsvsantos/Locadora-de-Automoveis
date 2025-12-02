using LocadoraDeAutomoveis.Application.Rentals.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Application;

[TestClass]
[TestCategory("Rental Application - Unit Tests")]
public sealed class DeleteRentalRequestHandlerTests
{
    private DeleteRentalRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteRentalRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteRentalRequestHandler>>();

        this.handler = new DeleteRentalRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteRental Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteRental_Successfully()
    {
        // Arrange
        Guid rentalId = Guid.NewGuid();
        DeleteRentalRequest request = new(rentalId);

        Rental rental = new(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddDays(5),
            0
        )
        { Id = rentalId };

        this.repositoryRentalMock
            .Setup(r => r.GetByIdAsync(rentalId))
            .ReturnsAsync(rental);

        this.repositoryRentalMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteRentalResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryRentalMock
            .Verify(r => r.GetByIdAsync(rentalId),
            Times.Once);

        this.repositoryRentalMock
            .Verify(r => r.DeleteAsync(request.Id),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
