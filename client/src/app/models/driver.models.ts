export interface DriverDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  document: string;
  licenseNumber: string;
  licenseValidity: Date;
}

export interface CreateDriverDto extends DriverDto {
  clientId: string;
  individualClientId: string | null;
}

export interface Driver extends DriverDto {
  id: string;
  client: DriverClientDto;
  isActive: boolean;
}

export interface ListDriversDto {
  quantity: number;
  drivers: Driver[];
}

export interface DriverDetailsApiDto {
  driver: Driver;
}

export type DriverDataPayload = ListDriversDto;

export interface DriverClientDto {
  id: string;
  fullName: string;
  type: string;
}

export interface ListDriverIndividualClientsDto {
  quantity: number;
  clients: DriverIndividualClientDto[];
}

export interface DriverIndividualClientDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  document: string;
  licenseNumber: string;
  licenseValidity: Date;
}
