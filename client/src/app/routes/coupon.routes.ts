import { Routes } from '@angular/router';
import { MostUsedCouponsComponent } from '../components/coupons/most-used/most-used-coupons.component';
import {
  listCouponsResolver,
  couponDetailsResolver,
  mostUsedCouponsResolver,
} from '../resolvers/coupon.resolvers';
import { ListCouponsComponent } from '../components/coupons/list/list-coupons.component';

export const couponRoutes: Routes = [
  {
    path: '',
    component: MostUsedCouponsComponent,
    resolve: { coupons: mostUsedCouponsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'list',
    component: ListCouponsComponent,
    resolve: { coupons: listCouponsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  //   { path: 'register', component: CreateCouponComponent },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateCouponComponent,
  //     resolve: { coupon: couponDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: DeleteCouponComponent,
  //     resolve: { coupon: couponDetailsResolver },
  //   },
];
