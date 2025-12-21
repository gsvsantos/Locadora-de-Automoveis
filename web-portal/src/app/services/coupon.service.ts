import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponseDto } from '../models/api.models';
import { Coupon, ListCouponsDto } from '../models/coupon.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class CouponService {
  private readonly apiUrl: string = environment.apiUrl + '/coupon';
  private readonly http: HttpClient = inject(HttpClient);

  public getAll(id: string): Observable<Coupon[]> {
    const url = `${this.apiUrl}/available/vehicle/${id}`;

    return this.http.get<ApiResponseDto>(url, {}).pipe(
      map(mapApiResponse<ListCouponsDto>),
      map((res) => res.coupons),
    );
  }
}
