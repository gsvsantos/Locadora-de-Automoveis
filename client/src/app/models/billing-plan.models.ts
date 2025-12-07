export interface BillingPlanDto {
  groupId: string;
  dailyBilling: DailyBillingDto;
  controlledBilling: ControlledBillingDto;
  freeBilling: FreeBillingDto;
}

export interface BillingPlan extends BillingPlanDto {
  id: string;
  name: string;
  isActive: boolean;
}

export interface ListBillingPlansDto {
  quantity: number;
  billingPlans: BillingPlan[];
}

export interface BillingPlanDetailsApiDto {
  billingPlan: {
    id: string;
    groupId: string;
    name: string;
    dailyBilling: DailyBillingDto;
    controlledBilling: ControlledBillingDto;
    freeBilling: FreeBillingDto;
    isActive: boolean;
  };
}

export type BillingPlanDataPayload = ListBillingPlansDto;

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
