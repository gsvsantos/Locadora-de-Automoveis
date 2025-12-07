import { Routes } from '@angular/router';
import { ListClientsComponent } from '../components/clients/list/list-clients.component';
import { listClientsResolver } from '../resolvers/client.resolvers';

export const clientRoutes: Routes = [
  {
    path: '',
    component: ListClientsComponent,
    resolve: { clients: listClientsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  //   {
  //     path: 'register',
  //     component: CreateClientComponent,
  //   },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateClientComponent,
  //     resolve: { client: clientDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: DeleteClientComponent,
  //     resolve: { client: clientDetailsResolver },
  //   },
];
