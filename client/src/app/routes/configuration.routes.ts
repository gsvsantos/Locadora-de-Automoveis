import { Routes } from '@angular/router';
import { ConfigureComponent } from '../components/configuration/configure/configure.component';
import { configurationDetailsResolver } from '../resolvers/configuration.resolvers';
import { ConfigurationDetailsComponent } from '../components/configuration/details/configuration-details.component';
import { AdminOnlyGuard } from '../guards/admin-only.guard';

export const configurationRoutes: Routes = [
  {
    path: 'configure',
    component: ConfigureComponent,
    resolve: { configuration: configurationDetailsResolver },
    canActivate: [AdminOnlyGuard],
  },
  {
    path: 'details',
    component: ConfigurationDetailsComponent,
    resolve: { configuration: configurationDetailsResolver },
  },
];
