import { Routes } from '@angular/router';
import { ListRentalsComponent } from '../components/rentals/list/list-rentals.component';
import { listRentalsResolver } from '../resolvers/rental.resolvers';
import { CreateRentalComponent } from '../components/rentals/create/create-rental.component';
import { listClientsResolver } from '../resolvers/client.resolvers';
import { listEmployeesResolver } from '../resolvers/employee.resolvers';
import { listDriversResolver } from '../resolvers/driver.resolvers';
import { listVehiclesResolver } from '../resolvers/vehicle.resolvers';
import { listExtrasResolver } from '../resolvers/extra.resolvers';

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
      //   coupons: listCouponsResolver, <- descomentar quando implementar & aprimorar html/component
      employees: listEmployeesResolver,
      drivers: listDriversResolver,
      vehicles: listVehiclesResolver,
      extras: listExtrasResolver,
    },
  },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateRentalComponent,
  //     resolve: { rental: rentalDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: DeleteRentalComponent,
  //     resolve: { rental: rentalDetailsResolver },
  //   },
];
