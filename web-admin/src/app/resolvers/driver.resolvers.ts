import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Driver } from '../models/driver.models';
import { DriverService } from '../services/driver.service';

export const listDriversResolver: ResolveFn<Driver[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Driver[]> => {
  const driverService: DriverService = inject(DriverService);

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

  return driverService.getAll(quantityFilter, isActiveFilter);
};

export const driverDetailsResolver: ResolveFn<Driver> = (route: ActivatedRouteSnapshot) => {
  const driverService: DriverService = inject(DriverService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return driverService.getById(id);
};
