using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Application;

[TestClass]
[TestCategory("Rental Application - Unit Tests")]
public sealed class GetByIdRentalRequestHandlerTests
{
    private GetByIdRentalRequestHandler handler = null!;

    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<GetByIdRentalRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<GetByIdRentalRequestHandler>>();

        this.handler = new GetByIdRentalRequestHandler(
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetRentalById (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetRentalById_Successfully()
    {
        // Arrange
        Guid rentalId = Guid.NewGuid();
        GetByIdRentalRequest request = new(rentalId);

        Rental rental = Builder<Rental>.CreateNew()
            .With(r => r.Id = rentalId)
            .With(r => r.StartDate = DateTimeOffset.Now)
            .With(r => r.ExpectedReturnDate = DateTimeOffset.Now.AddDays(5))
            .With(r => r.StartKm = 1000)
            .With(r => r.BaseRentalPrice = 500)
            .With(r => r.Client = Builder<Client>
                .CreateNew().With(c => c.FullName = "Cliente Teste").Build())
            .With(r => r.Driver = Builder<Driver
                >.CreateNew().With(d => d.FullName = "Motorista Teste").Build())
            .With(r => r.Vehicle = Builder<Vehicle>
                .CreateNew().With(v => v.LicensePlate = "ABC-1234").Build())
            .With(r => r.Employee = null)
            .With(r => r.RateServices = [])
            .Build();

        this.repositoryRentalMock
            .Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(rental);

        // Act
        Result<GetByIdRentalResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        ByIdRentalDto dto = result.Value.Rental;

        // Assert
        this.repositoryRentalMock
            .Verify(r => r.GetByIdAsync(request.Id), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value.Rental);
        Assert.AreEqual(rental.Id, dto.Id);
        Assert.AreEqual(rental.StartDate, dto.StartDate);
        Assert.AreEqual(rental.BaseRentalPrice, dto.BaseRentalPrice);
        Assert.AreEqual(rental.Client.FullName, dto.Client.FullName);
        Assert.AreEqual(rental.Driver.FullName, dto.Driver.FullName);
        Assert.AreEqual(rental.Vehicle.LicensePlate, dto.Vehicle.LicensePlate);
    }
    #endregion
}