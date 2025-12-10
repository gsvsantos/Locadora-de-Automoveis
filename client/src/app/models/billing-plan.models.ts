export interface BillingPlanDto {
  dailyBilling: DailyBillingDto;
  controlledBilling: ControlledBillingDto;
  freeBilling: FreeBillingDto;
}

export interface CreateBillingPlanDto extends BillingPlanDto {
  groupId: string;
}

export interface BillingPlan extends BillingPlanDto {
  id: string;
  group: BillingPlanGroupDto;
  isActive: boolean;
}

export interface ListBillingPlansDto {
  quantity: number;
  billingPlans: BillingPlan[];
}

export interface BillingPlanDetailsApiDto {
  billingPlan: {
    id: string;
    group: BillingPlanGroupDto;
    dailyBilling: DailyBillingDto;
    controlledBilling: ControlledBillingDto;
    freeBilling: FreeBillingDto;
    isActive: boolean;
  };
}

export type BillingPlanDataPayload = ListBillingPlansDto;

export interface BillingPlanGroupDto {
  id: string;
  name: string;
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
