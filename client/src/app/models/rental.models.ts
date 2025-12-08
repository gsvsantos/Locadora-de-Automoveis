export interface RentalDto {
  startDate: Date;
  expectedReturnDate: Date;
  startKm: number;
  billingPlanType: string;
  estimatedKilometers: number;
}

export interface CreateRentalDto extends RentalDto {
  employeeId: string;
  clientId: string;
  driverId: string;
  vehicleId: string;
  couponId: string;
  rentalRentalExtrasIds: string[];
}

export interface Rental extends RentalDto {
  id: string;
  employee: RentalEmployeeDto;
  client: RentalClientDto;
  driver: RentalDriverDto;
  vehicle: RentalVehicleDto;
  coupon: RentalCouponDto;
  returnDate: Date;
  baseRentalPrice: number;
  finalPrice: number;
  rentalExtrasQuantity: number;
  isActive: boolean;
}

export interface ListRentalsDto {
  quantity: number;
  rentals: Rental[];
}

export interface RentalDetailsDto extends Rental {
  rentalExtras: RentalExtra[];
}

export interface RentalDetailsApiDto {
  rental: RentalDetailsDto;
}

export type RentalDataPayload = ListRentalsDto;

export interface RentalExtra {
  id: string;
  name: string;
}

export interface RentalEmployeeDto {
  id: string;
  fullName: string;
  isActive: boolean;
}

export interface RentalClientDto {
  id: string;
  fullName: string;
  isActive: boolean;
}

export interface RentalDriverDto {
  id: string;
  fullName: string;
  isActive: boolean;
}

export interface RentalVehicleDto {
  id: string;
  licensePlate: string;
  isActive: boolean;
}

export interface RentalCouponDto {
  id: string;
  name: string;
  partner: RentalCouponPartnerDto;
  isActive: boolean;
}

export interface RentalCouponPartnerDto {
  id: string;
  fullName: string;
}

export interface ListRentalIndividualClientsDto {
  quantity: number;
  clients: RentalIndividualClientDto[];
}

export interface RentalIndividualClientDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  document: string;
  licenseNumber: string;
  licenseValidity: Date;
}
