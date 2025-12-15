import { BillingPlanDto, BillingPlanType } from './billing-plan.models';
import { Coupon } from './coupon.models';
import { Extra } from './extra.models';

export interface RentalDto {
  startDate: Date;
  expectedReturnDate: Date;
  startKm: number;
  billingPlanType: BillingPlanType;
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
  coupon: Coupon;
  billingPlan: BillingPlanDto;
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
  rentalExtras: Extra[];
}

export interface RentalDetailsApiDto {
  rental: RentalDetailsDto;
}

export interface ReturnRentalDto {
  endKm: number;
  fuelLevel: string;
}

export type RentalDataPayload = ListRentalsDto;

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
  fuelType: string;
  fuelTankCapacity: number;
  isActive: boolean;
}

export const FUEL_LEVEL_PERCENTAGE: Record<string, number> = {
  Empty: 0,
  Quarter: 25,
  Half: 50,
  ThreeQuarters: 75,
  Full: 100,
};

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
