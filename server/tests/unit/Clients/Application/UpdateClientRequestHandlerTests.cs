using LocadoraDeAutomoveis.Application.Clients.Commands.Update;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class UpdateClientRequestHandlerTests : UnitTestBase
{
    private UpdateClientRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<IValidator<Client>> validatorMock = null!;
    private Mock<ILogger<UpdateClientRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.validatorMock = new Mock<IValidator<Client>>();
        this.loggerMock = new Mock<ILogger<UpdateClientRequestHandler>>();

        this.handler = new UpdateClientRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryClientMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateClient Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateClient_Successfully()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "000.000.000-01",
            new(
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33)
        )
        { Id = clientId };

        this.repositoryClientMock
            .Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([client]);

        UpdateClientRequest request = new(
            clientId,
            "Ricardo ED",
            "ricardoED@gmail.com",
            "(51) 90000-0002",
            "SA",
            "CarazinhoED",
            "MarcondesED",
            "Edi MarcondesED",
            11,
            0,
            "000.000.000-02"
        );

        Guid updatedClientId = Guid.NewGuid();
        Address address = new(
            request.State,
            request.City,
            request.Neighborhood,
            request.Street,
            request.Number
        );

        Client updatedClient = new(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Document,
            address
        )
        { Id = updatedClientId };

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<Client>(),
                CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        // Act
        Result<UpdateClientResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.IsAny<Client>(),
                CancellationToken.None
                ), Times.Once
            );

        this.repositoryClientMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryClientMock
            .Verify(r => r.UpdateAsync(
                clientId,
                It.IsAny<Client>()), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
