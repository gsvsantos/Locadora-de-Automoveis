import { ClientProfile, ClientProfileApiDto } from './../models/account.models';
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapApiResponse } from '../utils/map-api-response';
import { ApiResponseDto } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/account`;

  public getProfile(): Observable<ClientProfile> {
    const url: string = `${this.apiUrl}/profile`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ClientProfileApiDto>),
      map((apiDto: ClientProfileApiDto) => this.mapClientProfileFromApi(apiDto.client)),
    );
  }

  private mapClientProfileFromApi(apiClientProfile: ClientProfileApiDto['client']): ClientProfile {
    return {
      id: apiClientProfile.id,
      fullName: apiClientProfile.fullName,
      email: apiClientProfile.email,
      phoneNumber: apiClientProfile.phoneNumber,
      document: apiClientProfile.document,
    };
  }
}
