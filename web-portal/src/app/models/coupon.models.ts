import { Partner } from './partner.models';

export interface CouponDto {
  name: string;
}

export interface Coupon extends CouponDto {
  id: string;
  partner: Partner;
  discountValue: number;
  expirationDate: Date;
  isActive: boolean;
}

export interface ListCouponsDto {
  quantity: number;
  coupons: Coupon[];
}
