using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Partners.Application;

[TestClass]
[TestCategory("Partner Application - Unit Tests")]
public sealed class GetAllPartnerRequestHandlerTests : UnitTestBase
{
    private GetAllPartnerRequestHandler handler = null!;

    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<ILogger<GetAllPartnerRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.loggerMock = new Mock<ILogger<GetAllPartnerRequestHandler>>();

        this.handler = new GetAllPartnerRequestHandler(
            this.mapper,
            this.repositoryPartnerMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllPartners Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllPartners_Successfully()
    {
        // Arrange
        List<Partner> partners = Builder<Partner>.CreateListOfSize(10).Build().ToList();

        this.repositoryPartnerMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(partners);

        GetAllPartnerRequest request = new(null, null);

        // Act
        Result<GetAllPartnerResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<GetAllPartnerDto> partnersDto = [.. result.Value.Partners];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(partners.Count, partnersDto.Count);

        for (int i = 0; i < partners.Count; i++)
        {
            for (int j = 0; j < partnersDto.Count; j++)
            {
                if (partners[i].Id == partnersDto[j].Id)
                {
                    Assert.AreEqual(partners[i].FullName, partnersDto[j].FullName);
                }
            }
        }
    }
    #endregion
}
