import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import {
  PartnerDto,
  PartnerDetailsApiDto,
  ListPartnersDto,
  PartnerDetailsDto,
  ListPartnerDto,
} from '../models/partner.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class PartnerService {
  private readonly apiUrl: string = environment.apiUrl + '/partner';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: PartnerDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: PartnerDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<PartnerDetailsDto> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<PartnerDetailsApiDto>),
      map((apiDto: PartnerDetailsApiDto) => this.mapPartnerFromApi(apiDto.partner)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<ListPartnerDto[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListPartnersDto>),
      map((res) => res.partners),
    );
  }

  private mapPartnerFromApi(apiPartner: PartnerDetailsApiDto['partner']): PartnerDetailsDto {
    return {
      coupons: apiPartner.coupons,
      id: apiPartner.id,
      fullName: apiPartner.fullName,
      isActive: apiPartner.isActive,
    };
  }
}
