import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { MostUsedCouponDto, Coupon } from '../models/coupon.models';
import { CouponService } from '../services/coupon.service';

export const mostUsedCouponsResolver: ResolveFn<MostUsedCouponDto[]> = (): Observable<
  MostUsedCouponDto[]
> => {
  const couponService: CouponService = inject(CouponService);

  return couponService.getMostUsed();
};

export const listCouponsResolver: ResolveFn<Coupon[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Coupon[]> => {
  const couponService: CouponService = inject(CouponService);

  const rawQuantity: string | null = route.queryParamMap.get('quantity');
  const rawIsActive: string | null = route.queryParamMap.get('isActive');

  const quantityFilter: number | undefined =
    rawQuantity !== null && rawQuantity.trim() !== '' ? Number(rawQuantity) : undefined;

  let isActiveFilter: boolean | undefined;

  switch (rawIsActive) {
    case 'true':
      isActiveFilter = true;
      break;
    case 'false':
      isActiveFilter = false;
      break;
    default:
      isActiveFilter = undefined;
      break;
  }

  return couponService.getAll(quantityFilter, isActiveFilter);
};

export const couponDetailsResolver: ResolveFn<Coupon> = (route: ActivatedRouteSnapshot) => {
  const couponService = inject(CouponService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return couponService.getById(id);
};
