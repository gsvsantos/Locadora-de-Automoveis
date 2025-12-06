import { Routes } from '@angular/router';
import { ListGroupsComponent } from '../components/groups/list/list-groups.component';
import { listGroupsResolver } from '../resolvers/group.resolvers';
import { CreateGroupComponent } from '../components/groups/create/create-group.component';

export const groupRoutes: Routes = [
  {
    path: '',
    component: ListGroupsComponent,
    resolve: { groups: listGroupsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  { path: 'register', component: CreateGroupComponent },
];
