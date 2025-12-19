import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Observable } from 'rxjs';
import { Coupon } from '../models/coupon.models';
import { CouponService } from '../services/coupon.service';

export const listCouponsResolver: ResolveFn<Coupon[]> = (): Observable<Coupon[]> => {
  const couponService: CouponService = inject(CouponService);

  return couponService.getAll();
};
