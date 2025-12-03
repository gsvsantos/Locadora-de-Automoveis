using LocadoraDeAutomoveis.Application.Partners.Commands.Update;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Partners.Application;

[TestClass]
[TestCategory("Partner Application - Unit Tests")]
public sealed class UpdatePartnerRequestHandlerTests : UnitTestBase
{
    private UpdatePartnerRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<IValidator<Partner>> validatorMock = null!;
    private Mock<ILogger<UpdatePartnerRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.validatorMock = new Mock<IValidator<Partner>>();
        this.loggerMock = new Mock<ILogger<UpdatePartnerRequestHandler>>();

        this.handler = new UpdatePartnerRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryPartnerMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdatePartner Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdatePartner_Successfully()
    {
        // Arrange
        Guid partnerId = Guid.NewGuid();
        UpdatePartnerRequest request = new(partnerId, "G2A");

        Partner partner = new(
            "MACHINIMA"
        )
        { Id = partnerId };

        this.repositoryPartnerMock
            .Setup(r => r.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        Partner updatedPartner = new(
            request.FullName
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Partner>(p =>
                    p.FullName == updatedPartner.FullName
                    ), CancellationToken.None
                )
            ).ReturnsAsync(new ValidationResult());

        this.repositoryPartnerMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([partner]);

        this.repositoryPartnerMock
            .Setup(r => r.UpdateAsync(request.Id, updatedPartner))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<UpdatePartnerResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryPartnerMock
            .Verify(r => r.GetByIdAsync(partnerId),
            Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Partner>(p =>
                    p.FullName == updatedPartner.FullName
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryPartnerMock
            .Verify(r => r.GetAllAsync(),
            Times.Once);

        this.repositoryPartnerMock
            .Verify(r => r.UpdateAsync(
                request.Id,
                It.Is<Partner>(p =>
                    p.FullName == updatedPartner.FullName
                    )
                ), Times.Once
            );

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(partnerId, result.Value.Id);
    }
    #endregion
}
