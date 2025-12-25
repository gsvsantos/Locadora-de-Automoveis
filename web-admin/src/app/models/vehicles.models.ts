import { Group } from './group.models';

export interface VehicleDto {
  licensePlate: string;
  brand: string;
  color: string;
  model: string;
  fuelType: FuelType;
  fuelTankCapacity: number;
  kilometers: number;
  year: number;
  image?: File;
  group: Group;
}

export interface Vehicle extends VehicleDto {
  id: string;
  isActive: boolean;
}

export interface ListVehiclesDto {
  quantity: number;
  vehicles: Vehicle[];
}

export interface VehicleDetailsApiDto {
  vehicle: Vehicle;
}

export type VehicleDataPayload = ListVehiclesDto;

export type FuelType = 'Gasoline' | 'Gas' | 'Diesel' | 'Alcohol';
