import { Routes } from '@angular/router';
import { ConfigureComponent } from '../components/configuration/configure/configure.component';
import { configurationDetailsResolver } from '../resolvers/configuration.resolvers';
import { ConfigurationDetailsComponent } from '../components/configuration/details/configuration-details.component';
import { adminOnlyGuard } from '../guards/admin-only.guard';
import { ConfigurationMenuComponent } from '../components/configuration/menu/configuration-menu.component';

export const configurationRoutes: Routes = [
  { path: '', component: ConfigurationMenuComponent },
  {
    path: 'configure',
    component: ConfigureComponent,
    resolve: { configuration: configurationDetailsResolver },
    canActivate: [adminOnlyGuard],
  },
  {
    path: 'details',
    component: ConfigurationDetailsComponent,
    resolve: { configuration: configurationDetailsResolver },
  },
];
