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
