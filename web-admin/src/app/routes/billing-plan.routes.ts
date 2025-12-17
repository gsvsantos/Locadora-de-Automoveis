import { Routes } from '@angular/router';
import { ListBillingPlansComponent } from '../components/billing-plans/list/list-billing-plans.component';
import {
  billingPlanDetailsResolver,
  listBillingPlansResolver,
} from '../resolvers/billing-plan.resolvers';
import { CreateBillingPlanComponent } from '../components/billing-plans/create/create-billing-plan.component';
import { listGroupsResolver } from '../resolvers/group.resolvers';
import { UpdateBillingPlanComponent } from '../components/billing-plans/update/update-billing-plan.component';
import { DeleteBillingPlanComponent } from '../components/billing-plans/delete/delete-billing-plan.component';

export const billingPlanRoutes: Routes = [
  {
    path: '',
    component: ListBillingPlansComponent,
    resolve: { billingPlans: listBillingPlansResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'register',
    component: CreateBillingPlanComponent,
    resolve: { groups: listGroupsResolver },
  },
  {
    path: 'edit/:id',
    component: UpdateBillingPlanComponent,
    resolve: { billingPlan: billingPlanDetailsResolver, groups: listGroupsResolver },
  },
  {
    path: 'delete/:id',
    component: DeleteBillingPlanComponent,
    resolve: { billingPlan: billingPlanDetailsResolver },
  },
];
