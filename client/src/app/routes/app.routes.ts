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
  {
    path: 'drivers',
    loadChildren: () => import('../routes/driver.routes').then((route) => route.driverRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'extras',
    loadChildren: () => import('../routes/extra.routes').then((route) => route.extraRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'rentals',
    loadChildren: () => import('../routes/rental.routes').then((route) => route.rentalRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'configuration',
    loadChildren: () =>
      import('../routes/configuration.routes').then((route) => route.configurationRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'partners',
    loadChildren: () => import('../routes/partner.routes').then((route) => route.partnerRoutes),
    canActivate: [authenticatedUserGuard],
  },
  {
    path: 'coupons',
    loadChildren: () => import('../routes/coupon.routes').then((route) => route.couponRoutes),
    canActivate: [authenticatedUserGuard],
  },
];
