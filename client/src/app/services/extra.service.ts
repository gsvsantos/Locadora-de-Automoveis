import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import { ExtraDto, Extra, ExtraDetailsApiDto, ListExtrasDto } from '../models/extra.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class ExtraService {
  private readonly apiUrl: string = environment.apiUrl + '/rental-extras';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: ExtraDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: ExtraDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<Extra> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ExtraDetailsApiDto>),
      map((apiDto: ExtraDetailsApiDto) => this.mapExtraFromApi(apiDto.extra)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Extra[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListExtrasDto>),
      map((res) => res.extras),
    );
  }

  private mapExtraFromApi(apiExtra: ExtraDetailsApiDto['extra']): Extra {
    return {
      id: apiExtra.id,
      name: apiExtra.name,
      price: apiExtra.price,
      isDaily: apiExtra.isDaily,
      type: apiExtra.type,
      isActive: apiExtra.isActive,
    };
  }
}
