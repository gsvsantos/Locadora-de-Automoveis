import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { Tenant, ListTenantsDto } from '../models/admin.models';
import { ApiResponseDto } from '../models/api.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly apiUrl: string = environment.apiUrl + '/admin';

  public getTenants(): Observable<Tenant[]> {
    const url = `${this.apiUrl}/tenants`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ListTenantsDto>),
      map((res) => res.tenants),
    );
  }
}
