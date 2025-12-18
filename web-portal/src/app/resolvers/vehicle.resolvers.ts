import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { VehicleService } from '../services/vehicle.service';
import { PagedResult } from '../models/paged-result.model';
import { Vehicle } from '../models/vehicle.model';

export const availableVehiclesResolver: ResolveFn<PagedResult<Vehicle>> = (
  route: ActivatedRouteSnapshot,
): Observable<PagedResult<Vehicle>> => {
  const vehicleService = inject(VehicleService);

  const page = Number(route.queryParamMap.get('page')) || 1;
  const pageSize = Number(route.queryParamMap.get('pageSize')) || 9;
  const term = route.queryParamMap.get('term') || undefined;
  const groupId = route.queryParamMap.get('groupId') || undefined;
  const fuelType = route.queryParamMap.get('fuelType') || undefined;

  return vehicleService.getAvailableVehicles(page, pageSize, term, groupId, fuelType);
};
