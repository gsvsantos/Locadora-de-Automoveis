namespace LocadoraDeAutomoveis.Application.Configurations.Commands.Details;

public record DetailsResponse(ConfigurationDto Configuration);

public record ConfigurationDto(
    Guid Id,
    decimal GasolinePrice,
    decimal GasPrice,
    decimal DieselPrice,
    decimal AlcoholPrice
);
