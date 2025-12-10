import { Routes } from '@angular/router';
import { ListClientsComponent } from '../components/clients/list/list-clients.component';
import { clientDetailsResolver, listClientsResolver } from '../resolvers/client.resolvers';
import { CreateClientComponent } from '../components/clients/create/create-client.component';
import { UpdateClientComponent } from '../components/clients/update/update-client.component';
import { DeleteClientComponent } from '../components/clients/delete/delete-client.component';

export const clientRoutes: Routes = [
  {
    path: '',
    component: ListClientsComponent,
    resolve: { clients: listClientsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'register',
    component: CreateClientComponent,
  },
  {
    path: 'edit/:id',
    component: UpdateClientComponent,
    resolve: { client: clientDetailsResolver },
  },
  {
    path: 'delete/:id',
    component: DeleteClientComponent,
    resolve: { client: clientDetailsResolver },
  },
];
