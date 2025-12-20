import { Routes } from '@angular/router';
import { CreateSelfComponent } from '../components/rentals/create-self/create-self.component';
import { vehicleDetailsResolver } from '../resolvers/vehicle.resolvers';
import { listCouponsResolver } from '../resolvers/coupon.resolvers';
import { listDriversResolver } from '../resolvers/driver.resolvers';
import { listExtrasResolver } from '../resolvers/extra.resolvers';

export const rentalRoutes: Routes = [
  // {
  //   path: '',
  //   component: ListRentalsComponent,
  //   resolve: { rentals: listRentalsResolver },
  //   runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  // },
  {
    path: 'new/:id',
    component: CreateSelfComponent,
    resolve: {
      vehicle: vehicleDetailsResolver,
      coupons: listCouponsResolver,
      drivers: listDriversResolver,
      extras: listExtrasResolver,
    },
  },
];
