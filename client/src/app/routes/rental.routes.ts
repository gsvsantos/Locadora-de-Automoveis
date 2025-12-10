import { Routes } from '@angular/router';
import { ListRentalsComponent } from '../components/rentals/list/list-rentals.component';
import { listRentalsResolver, rentalDetailsResolver } from '../resolvers/rental.resolvers';
import { CreateRentalComponent } from '../components/rentals/create/create-rental.component';
import { listClientsResolver } from '../resolvers/client.resolvers';
import { listEmployeesResolver } from '../resolvers/employee.resolvers';
import { listDriversResolver } from '../resolvers/driver.resolvers';
import { listVehiclesResolver } from '../resolvers/vehicle.resolvers';
import { listExtrasResolver } from '../resolvers/extra.resolvers';
import { UpdateRentalComponent } from '../components/rentals/update/update-rental.component';
import { DeleteRentalComponent } from '../components/rentals/delete/delete-rental.component';
import { ReturnRentalComponent } from '../components/rentals/return/return-rental.component';
import { listCouponsResolver } from '../resolvers/coupon.resolvers';

export const rentalRoutes: Routes = [
  {
    path: '',
    component: ListRentalsComponent,
    resolve: { rentals: listRentalsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'register',
    component: CreateRentalComponent,
    resolve: {
      clients: listClientsResolver,
      coupons: listCouponsResolver,
      employees: listEmployeesResolver,
      drivers: listDriversResolver,
      vehicles: listVehiclesResolver,
      extras: listExtrasResolver,
    },
  },
  {
    path: 'edit/:id',
    component: UpdateRentalComponent,
    resolve: {
      rental: rentalDetailsResolver,
      clients: listClientsResolver,
      coupons: listCouponsResolver,
      employees: listEmployeesResolver,
      drivers: listDriversResolver,
      vehicles: listVehiclesResolver,
      extras: listExtrasResolver,
    },
  },
  {
    path: 'delete/:id',
    component: DeleteRentalComponent,
    resolve: { rental: rentalDetailsResolver },
  },
  {
    path: 'return/:id',
    component: ReturnRentalComponent,
    resolve: { rental: rentalDetailsResolver },
  },
];
