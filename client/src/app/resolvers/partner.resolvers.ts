import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { ListPartnerDto, PartnerDetailsDto } from '../models/partner.models';
import { PartnerService } from '../services/partner.service';

export const listPartnersResolver: ResolveFn<ListPartnerDto[]> = (
  route: ActivatedRouteSnapshot,
): Observable<ListPartnerDto[]> => {
  const partnerService: PartnerService = inject(PartnerService);

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

  return partnerService.getAll(quantityFilter, isActiveFilter);
};

export const partnerDetailsResolver: ResolveFn<PartnerDetailsDto> = (
  route: ActivatedRouteSnapshot,
) => {
  const partnerService = inject(PartnerService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return partnerService.getById(id);
};
