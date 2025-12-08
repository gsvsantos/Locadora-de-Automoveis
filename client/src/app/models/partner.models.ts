import { Coupon } from './coupon.models';

export interface PartnerDto {
  fullName: string;
}

export interface Partner extends PartnerDto {
  id: string;
  isActive: boolean;
}

export interface ListPartnersDto {
  quantity: number;
  partners: ListPartnerDto[];
}

export interface ListPartnerDto extends Partner {
  couponsQuantity: number;
}

export interface PartnerDetailsDto extends Partner {
  coupons: Coupon[];
}

export interface PartnerDetailsApiDto {
  partner: PartnerDetailsDto;
}

export type PartnerDataPayload = ListPartnersDto;
