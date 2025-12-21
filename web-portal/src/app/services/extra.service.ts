import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponseDto } from '../models/api.models';
import { Extra, ListExtrasDto } from '../models/extra.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class ExtraService {
  private readonly apiUrl: string = environment.apiUrl + '/rental-extras';
  private readonly http: HttpClient = inject(HttpClient);

  public getAll(id: string): Observable<Extra[]> {
    const url = `${this.apiUrl}/available/vehicle/${id}`;

    return this.http.get<ApiResponseDto>(url, {}).pipe(
      map(mapApiResponse<ListExtrasDto>),
      map((res) => res.extras),
    );
  }
}
