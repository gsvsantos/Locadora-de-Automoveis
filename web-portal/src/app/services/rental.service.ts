import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import {
  RentalDetailsApiDto,
  RentalDetailsDto,
  ListClientRentalDto,
  ClientRentalDto,
  CreateSelfRentalDto,
  MyRentalStatusDto,
} from '../models/rental.models';
import { mapApiResponse } from '../utils/map-api-response';
import { PagedResult } from '../models/paged-result.models';

@Injectable({
  providedIn: 'root',
})
export class RentalService {
  private readonly apiUrl: string = environment.apiUrl + '/rental';
  private readonly http: HttpClient = inject(HttpClient);

  public createSelfRental(registerModel: CreateSelfRentalDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/self`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public getById(id: string): Observable<RentalDetailsDto> {
    const url: string = `${this.apiUrl}/me/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<RentalDetailsApiDto>),
      map((apiDto: RentalDetailsApiDto) => this.mapRentalFromApi(apiDto.rental)),
    );
  }

  public getAll(): Observable<PagedResult<ClientRentalDto>> {
    const url = `${this.apiUrl}/me`;

    return this.http.get<ApiResponseDto>(url, {}).pipe(
      map(mapApiResponse<ListClientRentalDto>),
      map((res) => res.rentals),
    );
  }

  public getMyRentalStatus(): Observable<MyRentalStatusDto> {
    const url = `${this.apiUrl}/me/status`;

    return this.http.get<ApiResponseDto>(url).pipe(map(mapApiResponse<MyRentalStatusDto>));
  }

  private mapRentalFromApi(apiRental: RentalDetailsApiDto['rental']): RentalDetailsDto {
    return {
      rentalExtras: apiRental.rentalExtras,
      id: apiRental.id,
      employee: apiRental.employee,
      client: apiRental.client,
      driver: apiRental.driver,
      vehicle: apiRental.vehicle,
      coupon: apiRental.coupon,
      startDate: apiRental.startDate,
      expectedReturnDate: apiRental.expectedReturnDate,
      startKm: apiRental.startKm,
      billingPlanType: apiRental.billingPlanType,
      billingPlan: apiRental.billingPlan,
      returnDate: apiRental.returnDate,
      rentalReturn: apiRental.rentalReturn,
      baseRentalPrice: apiRental.baseRentalPrice,
      finalPrice: apiRental.finalPrice,
      estimatedKilometers: apiRental.estimatedKilometers,
      rentalExtrasQuantity: apiRental.rentalExtrasQuantity,
      isActive: apiRental.isActive,
    };
  }
}
