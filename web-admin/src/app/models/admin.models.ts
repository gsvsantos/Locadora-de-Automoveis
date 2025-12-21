import { AuthApiResponse } from './auth.models';

export interface Tenant {
  tenantId: string;
  adminUserId: string;
  displayName: string;
  email: string;
  roles: string[];
}

export interface ListTenantsDto {
  quantity: number;
  tenants: Tenant[];
}

export type ImpersonateTenantApiResponse =
  | { success: true; data: { accessToken: AuthApiResponse } }
  | { success: false; errors: string[] };
