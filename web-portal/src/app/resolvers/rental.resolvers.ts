import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';
import { ClientRentalDto, RentalDetailsDto } from '../models/rental.models';
import { RentalService } from '../services/rental.service';
import { PagedResult } from '../models/paged-result.models';

export const listRentalsResolver: ResolveFn<PagedResult<ClientRentalDto>> = (): Observable<
  PagedResult<ClientRentalDto>
> => {
  const rentalService: RentalService = inject(RentalService);

  return rentalService.getAll();
};

export const rentalDetailsResolver: ResolveFn<RentalDetailsDto> = (
  route: ActivatedRouteSnapshot,
) => {
  const rentalService: RentalService = inject(RentalService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return rentalService.getById(id);
};
