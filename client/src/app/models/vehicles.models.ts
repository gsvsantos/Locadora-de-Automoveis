import { Group } from './group.models';

export interface VehicleDto {
  licensePlate: string;
  brand: string;
  color: string;
  model: string;
  fuelType: string;
  fuelTankCapacity: number;
  year: number;
  photoPath: string;
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
