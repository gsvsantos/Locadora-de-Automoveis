import { Routes } from '@angular/router';
import { authenticatedUserGuard } from '../guards/authenticated-user.guard';
import { availableVehiclesResolver } from '../resolvers/vehicle.resolvers';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'auth/login',
    pathMatch: 'full',
  },
  {
    path: 'auth',
    loadChildren: () => import('../routes/auth.routes').then((route) => route.authRoutes),
  },
  {
    path: 'home',
    loadComponent: () =>
      import('../components/home/home.component').then((component) => component.Home),
    resolve: {
      pagedVehicles: availableVehiclesResolver,
    },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'profile',
    loadChildren: () => import('../routes/profile.routes').then((route) => route.profileRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: '**',
    redirectTo: 'home',
  },
];
