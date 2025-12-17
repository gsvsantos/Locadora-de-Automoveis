import { AdminService } from './../services/admin.service';
import { Tenant } from '../models/admin.models';
import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';

export const listTenantsResolver: ResolveFn<Tenant[]> = (): Observable<Tenant[]> => {
  const adminService: AdminService = inject(AdminService);

  return adminService.getTenants();
};
