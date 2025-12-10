import { Routes } from '@angular/router';
import { ListDriversComponent } from '../components/drivers/list/list-drivers.component';
import { driverDetailsResolver, listDriversResolver } from '../resolvers/driver.resolvers';
import { CreateDriverComponent } from '../components/drivers/create/create-driver.component';
import { listClientsResolver } from '../resolvers/client.resolvers';
import { DeleteDriverComponent } from '../components/drivers/delete/delete-driver.component';

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
  {
    path: 'delete/:id',
    component: DeleteDriverComponent,
    resolve: { driver: driverDetailsResolver },
  },
];
