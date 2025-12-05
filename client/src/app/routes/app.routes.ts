import { Routes } from '@angular/router';
import { unknownUserGuard } from '../guards/unknown-user.guard';
import { authenticatedUserGuard } from '../guards/authenticated-user.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  {
    path: 'auth',
    loadChildren: () => import('../routes/auth.routes').then((route) => route.authRoutes),
    canActivate: [unknownUserGuard],
  },
  {
    path: 'home',
    loadComponent: () =>
      import('../components/home/home.component').then((component) => component.Home),
    canActivate: [authenticatedUserGuard],
  },
];
