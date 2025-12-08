import { Routes } from '@angular/router';
import { MostUsedCouponsComponent } from '../components/coupons/most-used/most-used-coupons.component';
import {
  listCouponsResolver,
  couponDetailsResolver,
  mostUsedCouponsResolver,
} from '../resolvers/coupon.resolvers';
import { ListCouponsComponent } from '../components/coupons/list/list-coupons.component';
import { CreateCouponComponent } from '../components/coupons/create/create-coupon.component';
import { listPartnersResolver } from '../resolvers/partner.resolvers';

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
  {
    path: 'register',
    component: CreateCouponComponent,
    resolve: { partners: listPartnersResolver },
  },
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
