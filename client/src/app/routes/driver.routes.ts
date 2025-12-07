import { Routes } from '@angular/router';
import { ListDriversComponent } from '../components/drivers/list/list-drivers.component';
import { listDriversResolver } from '../resolvers/driver.resolvers';
import { CreateDriverComponent } from '../components/drivers/create/create-driver.component';
import { listClientsResolver } from '../resolvers/client.resolvers';

export const driverRoutes: Routes = [
  {
    path: '',
    component: ListDriversComponent,
    resolve: { drivers: listDriversResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'register',
    component: CreateDriverComponent,
    resolve: { clients: listClientsResolver },
  },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateDriverComponent,
  //     resolve: { driver: driverDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: DeleteDriverComponent,
  //     resolve: { driver: driverDetailsResolver },
  //   },
];
