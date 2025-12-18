import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/paged-result.model';
import { Vehicle } from '../models/vehicle.model';

@Injectable({
  providedIn: 'root',
})
export class VehicleService {
  private readonly apiUrl: string = environment.apiUrl + '/vehicle';
  private readonly http: HttpClient = inject(HttpClient);

  public getAvailableVehicles(
    page: number = 1,
    pageSize: number = 10,
    term?: string,
    groupId?: string,
    fuelType?: string,
  ): Observable<PagedResult<Vehicle>> {
    let params = new HttpParams().set('pageNumber', page).set('pageSize', pageSize);

    if (term) params = params.set('term', term);
    if (groupId) params = params.set('groupId', groupId);
    if (fuelType) params = params.set('fuelType', fuelType);

    return this.http.get<PagedResult<Vehicle>>(`${this.apiUrl}/available`, { params });
  }
}
