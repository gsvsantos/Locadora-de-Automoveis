import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Group } from '../models/group.models';
import { GroupService } from '../services/group.service';

export const listGroupsResolver: ResolveFn<Group[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Group[]> => {
  const groupService: GroupService = inject(GroupService);

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

  return groupService.getAll(quantityFilter, isActiveFilter);
};

// export const groupDetailsResolver: ResolveFn<Group> = (route: ActivatedRouteSnapshot) => {
//   const groupService = inject(GroupService);

//   if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

//   const id: string = route.paramMap.get('id');

//   return groupService.getById(id);
// };
