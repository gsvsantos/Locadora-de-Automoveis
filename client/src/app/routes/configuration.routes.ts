import { Routes } from '@angular/router';
import { ConfigureComponent } from '../components/configuration/configure/configure.component';
import { configurationDetailsResolver } from '../resolvers/configuration.resolvers';
import { ConfigurationDetailsComponent } from '../components/configuration/details/configuration-details.component';
import { adminOnlyGuard } from '../guards/admin-only.guard';
import { ConfigurationMenuComponent } from '../components/configuration/menu/configuration-menu.component';
import { ProfileComponent } from '../components/configuration/profile/profile.component';

export const configurationRoutes: Routes = [
  { path: '', component: ConfigurationMenuComponent },
  {
    path: 'configure',
    component: ConfigureComponent,
    resolve: { configuration: configurationDetailsResolver },
    canActivate: [adminOnlyGuard],
  },
  {
    path: 'profile',
    component: ProfileComponent,
  },
  {
    path: 'details',
    component: ConfigurationDetailsComponent,
    resolve: { configuration: configurationDetailsResolver },
  },
];
