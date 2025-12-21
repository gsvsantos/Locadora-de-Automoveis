export interface BillingPlanDto {
  dailyBilling: DailyBillingDto;
  controlledBilling: ControlledBillingDto;
  freeBilling: FreeBillingDto;
}

export interface DailyBillingDto {
  dailyRate: number;
  pricePerKm: number;
}

export interface ControlledBillingDto {
  dailyRate: number;
  pricePerKmExtrapolated: number;
}

export interface FreeBillingDto {
  fixedRate: number;
}

export type BillingPlanType = 'Daily' | 'Controlled' | 'Free';
