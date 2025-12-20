import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';
import { Extra } from '../models/extra.models';
import { ExtraService } from '../services/extra.service';

export const listExtrasResolver: ResolveFn<Extra[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Extra[]> => {
  const extraService: ExtraService = inject(ExtraService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return extraService.getAll(id);
};
