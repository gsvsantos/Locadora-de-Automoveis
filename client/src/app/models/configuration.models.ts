export interface ConfigurationDto {
  gasolinePrice: number;
  gasPrice: number;
  dieselPrice: number;
  alcoholPrice: number;
}

export interface Configuration extends ConfigurationDto {
  id: string;
}

export interface ConfigutarionDetailsDto {
  configuration: Configuration;
}
