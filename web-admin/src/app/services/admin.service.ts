import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { Tenant, ListTenantsDto, ImpersonateTenantApiResponse } from '../models/admin.models';
import { ApiResponseDto } from '../models/api.models';
import { mapApiResponse } from '../utils/map-api-response';
import { AuthApiResponse } from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly apiUrl: string = environment.apiUrl + '/admin';

  public impersonateTenant(tenantId: string): Observable<AuthApiResponse> {
    const url: string = `${this.apiUrl}/imp?TenantId=${tenantId}`;

    return this.http.post<ImpersonateTenantApiResponse>(url, { withCredentials: true }).pipe(
      map((response) => {
        if (!response.success) {
          throw new Error(response.errors.join(', '));
        }
        return response.data.accessToken;
      }),
    );
  }

  public getTenants(): Observable<Tenant[]> {
    const url = `${this.apiUrl}/tenants`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ListTenantsDto>),
      map((res) => res.tenants),
    );
  }
}
