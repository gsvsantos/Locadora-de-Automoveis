import { Routes } from '@angular/router';
import { CreateSelfComponent } from '../components/rentals/create-self/create-self.component';
import { vehicleDetailsResolver } from '../resolvers/vehicle.resolvers';
import { listCouponsResolver } from '../resolvers/coupon.resolvers';
import { listDriversResolver } from '../resolvers/driver.resolvers';
import { listExtrasResolver } from '../resolvers/extra.resolvers';
import { listRentalsResolver, rentalDetailsResolver } from '../resolvers/rental.resolvers';
import { ListRentalsComponent } from '../components/rentals/list/list-rentals.component';
import { RentalDetailsComponent } from '../components/rentals/details/rental-details.component';

export const rentalRoutes: Routes = [
  {
    path: '',
    component: ListRentalsComponent,
    resolve: { rentals: listRentalsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
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
  {
    path: 'details/:id',
    component: RentalDetailsComponent,
    resolve: { rental: rentalDetailsResolver },
  },
];
