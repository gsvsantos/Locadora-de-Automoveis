import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { map, Observable, tap } from 'rxjs';
import { PagedResult } from '../models/paged-result.models';
import { ListVehiclesDto, Vehicle, VehicleDetailsApiDto } from '../models/vehicle.models';
import { ApiResponseDto } from '../models/api.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class VehicleService {
  private readonly apiUrl: string = environment.apiUrl + '/vehicle';
  private readonly http: HttpClient = inject(HttpClient);

  public getById(id: string): Observable<Vehicle> {
    const url: string = `${this.apiUrl}/available/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<VehicleDetailsApiDto>),
      map((apiDto: VehicleDetailsApiDto) => this.mapVehicleFromApi(apiDto.vehicle)),
    );
  }

  public getAvailableVehicles(
    page: number = 1,
    pageSize: number = 10,
    term?: string,
    groupId?: string,
    fuelType?: string,
  ): Observable<PagedResult<Vehicle>> {
    const url = `${this.apiUrl}/available`;
    let params = new HttpParams().set('pageNumber', page).set('pageSize', pageSize);

    if (term) params = params.set('term', term);
    if (groupId) params = params.set('groupId', groupId);
    if (fuelType) params = params.set('fuelType', fuelType);

    return this.http.get<ApiResponseDto>(url, { params }).pipe(
      map(mapApiResponse<ListVehiclesDto>),
      tap((res) => console.log(res)),
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
      year: apiVehicle.year,
      image: apiVehicle.image,
      group: apiVehicle.group,
      isActive: apiVehicle.isActive,
    };
  }
}
