using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Application;

[TestClass]
[TestCategory("Rental Application - Unit Tests")]
public sealed class GetAllRentalRequestHandlerTests : UnitTestBase
{
    private GetAllRentalRequestHandler handler = null!;

    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<GetAllRentalRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<GetAllRentalRequestHandler>>();

        this.handler = new GetAllRentalRequestHandler(
            this.mapper,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllRentals (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllRentals_Successfully()
    {
        // Arrange
        List<Rental> rentals = Builder<Rental>.CreateListOfSize(10).All()
            .With(r => r.Id = Guid.NewGuid())
            .With(r => r.Client = Builder<Client>
                .CreateNew().With(c => c.FullName = "Cliente Teste").Build())
            .With(r => r.Driver = Builder<Driver>
                .CreateNew().With(d => d.FullName = "Motorista Teste").Build())
            .With(r => r.Vehicle = Builder<Vehicle>
                .CreateNew().With(v => v.LicensePlate = "ABC-1234").Build())
            .With(r => r.Employee = null)
            .Build().ToList();

        this.repositoryRentalMock
            .Setup(r => r.GetAllAsync(true))
            .ReturnsAsync(rentals);

        GetAllRentalRequest request = new(null, null);

        // Act
        Result<GetAllRentalResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<RentalDto> rentalsDto = result.Value.Rentals.ToList();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(rentals.Count, rentalsDto.Count);

        for (int i = 0; i < rentals.Count; i++)
        {
            RentalDto? dto = rentalsDto.FirstOrDefault(d => d.Id == rentals[i].Id);

            Assert.IsNotNull(dto);
            Assert.AreEqual(rentals[i].StartDate, dto.StartDate);
            Assert.AreEqual(rentals[i].ExpectedReturnDate, dto.ExpectedReturnDate);
            Assert.AreEqual(rentals[i].BaseRentalPrice, dto.BaseRentalPrice);
            Assert.AreEqual(rentals[i].FinalPrice, dto.FinalPrice);
            Assert.AreEqual(rentals[i].Client.FullName, dto.Client.FullName);
            Assert.AreEqual(rentals[i].Driver.FullName, dto.Driver.FullName);
            Assert.AreEqual(rentals[i].Vehicle.LicensePlate, dto.Vehicle.LicensePlate);
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveRentals_Successfully()
    {
        // Arrange
        List<Rental> rentals = Builder<Rental>.CreateListOfSize(10).All()
            .With(r => r.Id = Guid.NewGuid())
            .With(r => r.Client = Builder<Client>
                .CreateNew().With(c => c.FullName = "Cliente Teste").Build())
            .With(r => r.Driver = Builder<Driver>
                .CreateNew().With(d => d.FullName = "Motorista Teste").Build())
            .With(r => r.Vehicle = Builder<Vehicle>
                .CreateNew().With(v => v.LicensePlate = "ABC-1234").Build())
            .With(r => r.Employee = null)
            .Build().ToList();

        this.repositoryRentalMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync(rentals.Take(5).ToList());

        GetAllRentalRequest request = new(5, null);

        // Act
        Result<GetAllRentalResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<RentalDto> rentalsDto = result.Value.Rentals.ToList();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, rentalsDto.Count);

        for (int i = 0; i < rentalsDto.Count; i++)
        {
            Rental entity = rentals[i];
            RentalDto dto = rentalsDto[i];

            Assert.AreEqual(entity.Id, dto.Id);
            Assert.AreEqual(entity.Client.FullName, dto.Client.FullName);
            Assert.AreEqual(entity.Vehicle.LicensePlate, dto.Vehicle.LicensePlate);
        }
    }
    #endregion
}