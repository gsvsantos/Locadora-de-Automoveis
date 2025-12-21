import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Vehicle } from '../models/vehicles.models';
import { VehicleService } from '../services/vehicle.service';

export const listVehiclesResolver: ResolveFn<Vehicle[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Vehicle[]> => {
  const vehicleService: VehicleService = inject(VehicleService);

  const rawQuantity: string | null = route.queryParamMap.get('quantity');
  const rawIsActive: string | null = route.queryParamMap.get('isActive');

  const quantityFilter: number | undefined =
    rawQuantity !== null && rawQuantity.trim() !== '' ? Number(rawQuantity) : undefined;

  let isActiveFilter: boolean | undefined;

  switch (rawIsActive) {
    case 'true':
      isActiveFilter = true;
      break;
    case 'false':
      isActiveFilter = false;
      break;
    default:
      isActiveFilter = undefined;
      break;
  }

  return vehicleService.getAll(quantityFilter, isActiveFilter);
};

export const vehicleDetailsResolver: ResolveFn<Vehicle> = (route: ActivatedRouteSnapshot) => {
  const vehicleService: VehicleService = inject(VehicleService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return vehicleService.getById(id);
};
