import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';
import { Group } from '../models/group.models';
import { GroupService } from '../services/group.service';

export const listDistinctGroupsResolver: ResolveFn<Group[]> = (): Observable<Group[]> => {
  const groupService: GroupService = inject(GroupService);

  return groupService.getAllDistinct();
};
