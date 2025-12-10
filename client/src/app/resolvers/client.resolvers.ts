import { inject } from "@angular/core";
import { ResolveFn, ActivatedRouteSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Client } from "../models/client.models";
import { ClientService } from "../services/client.service";

export const listClientsResolver: ResolveFn<Client[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Client[]> => {
  const clientService: ClientService = inject(ClientService);

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

  return clientService.getAll(quantityFilter, isActiveFilter);
};

export const clientDetailsResolver: ResolveFn<Client> = (
  route: ActivatedRouteSnapshot,
) => {
  const clientService: ClientService = inject(ClientService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return clientService.getById(id);
};
