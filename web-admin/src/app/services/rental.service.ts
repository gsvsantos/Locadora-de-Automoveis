import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import {
  Rental,
  RentalDetailsApiDto,
  ListRentalsDto,
  CreateRentalDto,
  RentalDetailsDto,
  RentalReturnDto,
} from '../models/rental.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class RentalService {
  private readonly apiUrl: string = environment.apiUrl + '/rental';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: CreateRentalDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: CreateRentalDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }
  public return(id: string, returnModel: RentalReturnDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/return/${id}`;

    return this.http.post<IdApiResponse>(url, returnModel);
  }

  public getById(id: string): Observable<RentalDetailsDto> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<RentalDetailsApiDto>),
      map((apiDto: RentalDetailsApiDto) => this.mapRentalFromApi(apiDto.rental)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Rental[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListRentalsDto>),
      map((res) => res.rentals),
    );
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
