import { Routes } from '@angular/router';
import { ListBillingPlansComponent } from '../components/billing-plans/list/list-billing-plans.component';
import { listBillingPlansResolver } from '../resolvers/billing-plan.resolvers';

export const billingPlanRoutes: Routes = [
  {
    path: '',
    component: ListBillingPlansComponent,
    resolve: { billingPlans: listBillingPlansResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  //   { path: 'register', component: CreateBillingPlanComponent },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateBillingPlanComponent,
  //     resolve: { billingPlan: billingPlanDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: DeleteBillingPlanComponent,
  //     resolve: { billingPlan: billingPlanDetailsResolver },
  //   },
];
