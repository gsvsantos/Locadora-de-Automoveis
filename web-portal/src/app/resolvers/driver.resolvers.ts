import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Driver } from '../models/driver.models';
import { DriverService } from '../services/driver.service';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';

export const listDriversResolver: ResolveFn<Driver[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Driver[]> => {
  const driverService: DriverService = inject(DriverService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return driverService.getAll(id);
};
