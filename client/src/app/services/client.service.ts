import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import { ClientDto, Client, ClientDetailsApiDto, ListClientsDto } from '../models/client.models';
import { mapApiResponse } from '../utils/map-api-response';
import { DriverIndividualClientDto, ListDriverIndividualClientsDto } from '../models/driver.models';

@Injectable({
  providedIn: 'root',
})
export class ClientService {
  private readonly apiUrl: string = environment.apiUrl + '/client';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: ClientDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: ClientDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<Client> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ClientDetailsApiDto>),
      map((apiDto: ClientDetailsApiDto) => this.mapClientFromApi(apiDto.client)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Client[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListClientsDto>),
      map((res) => res.clients),
    );
  }

  public getIndividuals(id: string): Observable<DriverIndividualClientDto[]> {
    const url = `${this.apiUrl}/business/${id}/individuals`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ListDriverIndividualClientsDto>),
      map((res) => res.clients),
    );
  }

  private mapClientFromApi(apiClient: ClientDetailsApiDto['client']): Client {
    return {
      id: apiClient.id,
      fullName: apiClient.fullName,
      email: apiClient.email,
      phoneNumber: apiClient.phoneNumber,
      document: apiClient.document,
      address: apiClient.address,
      type: apiClient.type,
      licenseNumber: apiClient.licenseNumber,
      isActive: apiClient.isActive,
    };
  }
}
