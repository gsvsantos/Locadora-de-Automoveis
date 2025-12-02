using LocadoraDeAutomoveis.Application.Partners.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Partners.Application;

[TestClass]
[TestCategory("Partner Application - Unit Tests")]
public sealed class DeletePartnerRequestHandlerTests
{
    private DeletePartnerRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<ILogger<DeletePartnerRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.loggerMock = new Mock<ILogger<DeletePartnerRequestHandler>>();

        this.handler = new DeletePartnerRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryPartnerMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeletePartner Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeletePartner_Successfully()
    {
        // Arrange
        Guid partnerId = Guid.NewGuid();
        DeletePartnerRequest request = new(partnerId);

        Partner partner = new(
            "G2A"
        )
        { Id = partnerId };

        this.repositoryPartnerMock
            .Setup(r => r.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        this.repositoryPartnerMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<DeletePartnerResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryPartnerMock
            .Verify(r => r.GetByIdAsync(partnerId),
            Times.Once);

        this.repositoryPartnerMock
            .Verify(r => r.DeleteAsync(request.Id), Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
