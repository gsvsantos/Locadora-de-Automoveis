import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';
import { Coupon } from '../models/coupon.models';
import { CouponService } from '../services/coupon.service';

export const listCouponsResolver: ResolveFn<Coupon[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Coupon[]> => {
  const couponService: CouponService = inject(CouponService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return couponService.getAll(id);
};
