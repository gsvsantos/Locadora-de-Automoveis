import { groupDetailsResolver } from './../resolvers/group.resolvers';
import { Routes } from '@angular/router';
import { ListGroupsComponent } from '../components/groups/list/list-groups.component';
import { listGroupsResolver } from '../resolvers/group.resolvers';
import { CreateGroupComponent } from '../components/groups/create/create-group.component';
import { UpdateGroupComponent } from '../components/groups/update/update-group.component';

export const groupRoutes: Routes = [
  {
    path: '',
    component: ListGroupsComponent,
    resolve: { groups: listGroupsResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  { path: 'register', component: CreateGroupComponent },
  {
    path: 'edit/:id',
    component: UpdateGroupComponent,
    resolve: { group: groupDetailsResolver },
  },
];
