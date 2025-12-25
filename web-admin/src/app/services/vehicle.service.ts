import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable, map } from 'rxjs';
import { ApiResponseDto, IdApiResponse } from '../models/api.models';
import { mapApiResponse } from '../utils/map-api-response';
import { Vehicle, VehicleDetailsApiDto, ListVehiclesDto } from '../models/vehicles.models';

@Injectable({
  providedIn: 'root',
})
export class VehicleService {
  private readonly apiUrl: string = environment.apiUrl + '/vehicle';
  private readonly http: HttpClient = inject(HttpClient);

  public register(data: FormData): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, data);
  }

  public update(id: string, data: FormData): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, data);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<Vehicle> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<VehicleDetailsApiDto>),
      map((apiDto: VehicleDetailsApiDto) => this.mapVehicleFromApi(apiDto.vehicle)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Vehicle[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListVehiclesDto>),
      map((res) => res.vehicles),
    );
  }

  private mapVehicleFromApi(apiVehicle: VehicleDetailsApiDto['vehicle']): Vehicle {
    return {
      id: apiVehicle.id,
      licensePlate: apiVehicle.licensePlate,
      brand: apiVehicle.brand,
      color: apiVehicle.color,
      model: apiVehicle.model,
      fuelType: apiVehicle.fuelType,
      fuelTankCapacity: apiVehicle.fuelTankCapacity,
      kilometers: apiVehicle.kilometers,
      year: apiVehicle.year,
      image: apiVehicle.image,
      group: apiVehicle.group,
      isActive: apiVehicle.isActive,
    };
  }
}
