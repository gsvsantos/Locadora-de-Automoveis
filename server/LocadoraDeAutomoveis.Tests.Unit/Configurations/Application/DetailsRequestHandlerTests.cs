using LocadoraDeAutomoveis.Application.Configurations.Commands.Details;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;

namespace LocadoraDeAutomoveis.Tests.Unit.Configurations.Application;

[TestClass]
[TestCategory("Configuration Application - Unit Tests")]
public sealed class DetailsRequestHandlerTests
{
    private DetailsRequestHandler handler = null!;

    private Mock<IRepositoryConfiguration> repositoryConfigurationMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<ILogger<DetailsRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryConfigurationMock = new Mock<IRepositoryConfiguration>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.loggerMock = new Mock<ILogger<DetailsRequestHandler>>();

        this.handler = new DetailsRequestHandler(
            this.repositoryConfigurationMock.Object,
            this.tenantProviderMock.Object,
            this.loggerMock.Object
        );
    }

    #region ConfigDetails Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetGroupById_Successfuly()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        DetailsRequest request = new();

        Configuration config = new(
            20m,
            40m,
            15m,
            12m
        );

        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        this.repositoryConfigurationMock
            .Setup(r => r.GetByTenantId(tenantId))
            .ReturnsAsync(config);

        // Act
        Result<DetailsResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        ConfigurationDto dto = result.Value.Configuration;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(config.Id, dto.Id);
        Assert.AreEqual(config.GasolinePrice, dto.GasolinePrice);
        Assert.AreEqual(config.GasPrice, dto.GasPrice);
        Assert.AreEqual(config.DieselPrice, dto.DieselPrice);
        Assert.AreEqual(config.AlcoholPrice, dto.AlcoholPrice);
    }
    #endregion
}
