import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import { DriverDto, Driver, DriverDetailsApiDto, ListDriversDto } from '../models/driver.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class DriverService {
  private readonly apiUrl: string = environment.apiUrl + '/driver';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: DriverDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: DriverDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<Driver> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<DriverDetailsApiDto>),
      map((apiDto: DriverDetailsApiDto) => this.mapDriverFromApi(apiDto.driver)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Driver[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListDriversDto>),
      map((res) => res.drivers),
    );
  }

  private mapDriverFromApi(apiDriver: DriverDetailsApiDto['driver']): Driver {
    return {
      id: apiDriver.id,
      fullName: apiDriver.fullName,
      email: apiDriver.email,
      phoneNumber: apiDriver.phoneNumber,
      document: apiDriver.document,
      licenseNumber: apiDriver.licenseNumber,
      licenseValidity: apiDriver.licenseValidity,
      client: apiDriver.client,
      isActive: apiDriver.isActive,
    };
  }
}
