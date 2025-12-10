import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import {
  CouponDto,
  Coupon,
  CouponDetailsApiDto,
  MostUsedCouponsDto,
  MostUsedCouponDto,
  ListCouponsDto,
} from '../models/coupon.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class CouponService {
  private readonly apiUrl: string = environment.apiUrl + '/coupon';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: CouponDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: CouponDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<Coupon> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<CouponDetailsApiDto>),
      map((apiDto: CouponDetailsApiDto) => this.mapCouponFromApi(apiDto.coupon)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Coupon[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListCouponsDto>),
      map((res) => res.coupons),
    );
  }

  public getMostUsed(): Observable<MostUsedCouponDto[]> {
    const url = `${this.apiUrl}/most-used`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<MostUsedCouponsDto>),
      map((res) => res.coupons),
    );
  }

  private mapCouponFromApi(apiCoupon: CouponDetailsApiDto['coupon']): Coupon {
    return {
      id: apiCoupon.id,
      name: apiCoupon.name,
      discountValue: apiCoupon.discountValue,
      expirationDate: apiCoupon.expirationDate,
      isActive: apiCoupon.isActive,
    };
  }
}
