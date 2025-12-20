import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';
import { ClientRentalDto } from '../models/rental.models';
import { RentalService } from '../services/rental.service';
import { PagedResult } from '../models/paged-result.models';

export const listRentalsResolver: ResolveFn<PagedResult<ClientRentalDto>> = (): Observable<
  PagedResult<ClientRentalDto>
> => {
  const rentalService: RentalService = inject(RentalService);

  return rentalService.getAll();
};
