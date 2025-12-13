import { Routes } from '@angular/router';
import { ListTenantsComponent } from '../components/admin/list/list-tenants.component';
import { listTenantsResolver } from '../resolvers/admin.resolvers';

export const adminRoutes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'tenants',
        component: ListTenantsComponent,
        resolve: { tenants: listTenantsResolver },
      },
      //   { path: 'impersonate', component: ImpersonateTenantComponent },
    ],
  },
];
