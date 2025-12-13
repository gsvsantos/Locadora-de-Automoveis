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
