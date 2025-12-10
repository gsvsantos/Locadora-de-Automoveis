export interface CouponDto {
  name: string;
}

export interface CreateCouponDto extends CouponDto {
  discountValue: number;
  expirationDate: Date;
  partnerId: string;
}

export interface Coupon extends CouponDto {
  id: string;
  discountValue: number;
  expirationDate: Date;
  isActive: boolean;
}

export interface ListCouponsDto {
  quantity: number;
  coupons: Coupon[];
}

export interface MostUsedCouponsDto {
  quantity: number;
  coupons: MostUsedCouponDto[];
}

export interface MostUsedCouponDto extends CouponDto {
  id: string;
  partnerName: string;
  usageCount: number;
  totalDiscountGiven: number;
}

export interface CouponDetailsApiDto {
  coupon: Coupon;
}

export type CouponDataPayload = MostUsedCouponsDto;
