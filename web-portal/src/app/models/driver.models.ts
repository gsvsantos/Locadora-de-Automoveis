export interface DriverDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  document: string;
  licenseNumber: string;
  licenseValidity: Date;
}

export interface Driver extends DriverDto {
  id: string;
  client: DriverClientDto;
  isActive: boolean;
}

export interface DriverClientDto {
  id: string;
  fullName: string;
  type: string;
}

export interface ListDriversDto {
  quantity: number;
  drivers: Driver[];
}
