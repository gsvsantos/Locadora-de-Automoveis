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
  {
    path: 'employees',
    loadChildren: () => import('../routes/employee.routes').then((route) => route.employeeRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'groups',
    loadChildren: () => import('../routes/group.routes').then((route) => route.groupRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'billing-plans',
    loadChildren: () =>
      import('../routes/billing-plan.routes').then((route) => route.billingPlanRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'vehicles',
    loadChildren: () => import('../routes/vehicle.routes').then((route) => route.vehicleRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'clients',
    loadChildren: () => import('../routes/client.routes').then((route) => route.clientRoutes),
    canActivate: [authenticatedUserGuard],
  },
];
