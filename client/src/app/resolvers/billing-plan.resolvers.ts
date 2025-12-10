import { inject } from '@angular/core';
import { ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { BillingPlan } from '../models/billing-plan.models';
import { BillingPlanService } from '../services/billing-plan.service';

export const listBillingPlansResolver: ResolveFn<BillingPlan[]> = (
  route: ActivatedRouteSnapshot,
): Observable<BillingPlan[]> => {
  const billingPlanService: BillingPlanService = inject(BillingPlanService);

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

  return billingPlanService.getAll(quantityFilter, isActiveFilter);
};

export const billingPlanDetailsResolver: ResolveFn<BillingPlan> = (
  route: ActivatedRouteSnapshot,
) => {
  const billingPlanService: BillingPlanService = inject(BillingPlanService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return billingPlanService.getById(id);
};
