using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.RentalExtras.Application;

[TestClass]
[TestCategory("RentalExtra Application - Unit Tests")]
public sealed class GetAllRentalExtraRequestHandlerTests : UnitTestBase
{
    private GetAllRentalExtraRequestHandler handler = null!;

    private Mock<IRepositoryRentalExtra> repositoryRentalExtraMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<ILogger<GetAllRentalExtraRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryRentalExtraMock = new Mock<IRepositoryRentalExtra>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.loggerMock = new Mock<ILogger<GetAllRentalExtraRequestHandler>>();

        this.handler = new GetAllRentalExtraRequestHandler(
            this.mapper,
            this.repositoryRentalExtraMock.Object,
            this.cacheMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllRentalExtras Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllBillingPlans_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<RentalExtra> rentalExtras = Builder<RentalExtra>.CreateListOfSize(10).All()
            .Do(v => v.Name = random.NextString(1, 5))
            .Do(v => v.Price = random.Decimal())
            .Build().ToList();

        this.repositoryRentalExtraMock
            .Setup(r => r.GetAllAsync(true))
            .ReturnsAsync(rentalExtras);

        GetAllRentalExtraRequest request = new(null, null);

        // Act
        Result<GetAllRentalExtraResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<RentalExtraDto> rentalExtrasDto = [.. result.Value.Extras];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(rentalExtras.Count, rentalExtrasDto.Count);

        for (int i = 0; i < rentalExtras.Count; i++)
        {
            for (int j = 0; j < rentalExtrasDto.Count; j++)
            {
                if (rentalExtras[i].Id == rentalExtrasDto[j].Id)
                {
                    Assert.AreEqual(rentalExtras[i].Name, rentalExtras[j].Name);
                    Assert.AreEqual(rentalExtras[i].Price, rentalExtras[j].Price);
                    Assert.AreEqual(rentalExtras[i].IsDaily, rentalExtras[j].IsDaily);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveBillingPlan_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<RentalExtra> rentalExtras = Builder<RentalExtra>.CreateListOfSize(10).All()
            .Do(v => v.Name = random.NextString(1, 5))
            .Do(v => v.Price = random.Decimal())
            .Build().ToList();

        this.repositoryRentalExtraMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. rentalExtras.Take(5)]);

        GetAllRentalExtraRequest request = new(5, null);

        // Act
        Result<GetAllRentalExtraResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<RentalExtraDto> rentalExtrasDto = [.. result.Value.Extras];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, rentalExtras.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, rentalExtrasDto.Count);

        for (int i = 0; i < rentalExtras.Count; i++)
        {
            for (int j = 0; j < rentalExtrasDto.Count; j++)
            {
                if (rentalExtras[i].Id == rentalExtrasDto[j].Id)
                {
                    Assert.AreEqual(rentalExtras[i].Name, rentalExtras[j].Name);
                    Assert.AreEqual(rentalExtras[i].Price, rentalExtras[j].Price);
                    Assert.AreEqual(rentalExtras[i].IsDaily, rentalExtras[j].IsDaily);
                }
            }
        }
    }
    #endregion
}
