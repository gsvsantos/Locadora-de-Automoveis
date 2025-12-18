import { Group } from './group.models';

export interface VehicleDto {
  licensePlate: string;
  brand: string;
  color: string;
  model: string;
  fuelType: FuelType;
  fuelTankCapacity: number;
  year: number;
  image?: File;
  group: Group;
}

export interface Vehicle extends VehicleDto {
  id: string;
  isActive: boolean;
}

export type FuelType = 'Gasoline' | 'Gas' | 'Diesel' | 'Alcohol';
