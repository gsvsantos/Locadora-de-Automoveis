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

export interface CombustivelApiDto {
  error: boolean;
  data_coleta: Date;
  moeda: string;
  precos: Prices;
}

export interface Prices {
  gasolina: States;
  diesel: States;
}

export interface States {
  br: string;
  al: string;
  am: string;
  ce: string;
  df: string;
  es: string;
  go: string;
  ma: string;
  mt: string;
  mg: string;
  pr: string;
  pa: string;
  pe: string;
  rs: string;
  rj: string;
  sc: string;
  sp: string;
  pb?: string;
}
