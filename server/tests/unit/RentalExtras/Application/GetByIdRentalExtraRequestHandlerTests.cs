using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetById;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.RentalExtras.Application;

[TestClass]
[TestCategory("RentalExtra Application - Unit Tests")]
public sealed class GetByIdRentalExtraRequestHandlerTests : UnitTestBase
{
    private GetByIdRentalExtraRequestHandler handler = null!;

    private Mock<IRepositoryRentalExtra> repositoryRentalExtraMock = null!;
    private Mock<ILogger<GetByIdRentalExtraRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryRentalExtraMock = new Mock<IRepositoryRentalExtra>();
        this.loggerMock = new Mock<ILogger<GetByIdRentalExtraRequestHandler>>();

        this.handler = new GetByIdRentalExtraRequestHandler(
            this.mapper,
            this.repositoryRentalExtraMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetRentalExtraById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetBillingPlansById_Successfuly()
    {
        // Arrange
        Guid rentalExtraId = Guid.NewGuid();
        GetByIdRentalExtraRequest request = new(
            rentalExtraId
        );

        RentalExtra rentalExtra = new(
            "GPS",
            20
        )
        { Id = rentalExtraId };
        rentalExtra.MarkAsFixed();

        this.repositoryRentalExtraMock
            .Setup(r => r.GetByIdAsync(rentalExtraId))
            .ReturnsAsync(rentalExtra);

        // Act
        Result<GetByIdRentalExtraResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        RentalExtraDto dto = result.Value.RentalExtra;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(rentalExtra.Id, dto.Id);
        Assert.AreEqual(rentalExtra.Name, dto.Name);
        Assert.AreEqual(rentalExtra.Price, dto.Price);
        Assert.AreEqual(rentalExtra.IsDaily, dto.IsDaily);
    }
    #endregion
}
