import { Routes } from '@angular/router';
import { authenticatedUserGuard } from '../guards/authenticated-user.guard';

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
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'account',
    loadChildren: () => import('./account.routes').then((route) => route.accountRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'rentals',
    loadChildren: () => import('./rental.routes').then((route) => route.rentalRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: '**',
    redirectTo: 'home',
  },
];
