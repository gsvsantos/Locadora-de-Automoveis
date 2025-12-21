import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponseDto } from '../models/api.models';
import { Driver, ListDriversDto } from '../models/driver.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class DriverService {
  private readonly apiUrl: string = environment.apiUrl + '/driver';
  private readonly http: HttpClient = inject(HttpClient);

  public getAll(id: string): Observable<Driver[]> {
    const url = `${this.apiUrl}/available/vehicle/${id}`;

    return this.http.get<ApiResponseDto>(url, {}).pipe(
      map(mapApiResponse<ListDriversDto>),
      map((res) => res.drivers),
    );
  }
}
