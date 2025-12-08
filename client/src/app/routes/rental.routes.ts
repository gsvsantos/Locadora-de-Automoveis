import { Routes } from '@angular/router';
import { ListRentalsComponent } from '../components/rentals/list/list-rentals.component';
import { listRentalsResolver } from '../resolvers/rental.resolvers';

export const rentalRoutes: Routes = [
  {
    path: '',
    component: ListRentalsComponent,
    resolve: { rentals: listRentalsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  //   {
  //     path: 'register',
  //     component: CreateRentalComponent,
  //     resolve: { clients: listClientsResolver },
  //   },
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
