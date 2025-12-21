import { Routes } from '@angular/router';
import { authenticatedUserGuard } from '../guards/authenticated-user.guard';
import { listVehiclesResolver } from '../resolvers/vehicle.resolvers';
import { listRentalsResolver } from '../resolvers/rental.resolvers';
import { listClientsResolver } from '../resolvers/client.resolvers';
import { mostUsedCouponsResolver } from '../resolvers/coupon.resolvers';
import { platformAdminOnlyGuard } from '../guards/platform-admin-only.guard';

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
    canMatch: [authenticatedUserGuard],
    resolve: {
      clients: listClientsResolver,
      coupons: mostUsedCouponsResolver,
      rentals: listRentalsResolver,
      vehicles: listVehiclesResolver,
    },
  },
  {
    path: 'employees',
    loadChildren: () => import('../routes/employee.routes').then((route) => route.employeeRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'groups',
    loadChildren: () => import('../routes/group.routes').then((route) => route.groupRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'billing-plans',
    loadChildren: () =>
      import('../routes/billing-plan.routes').then((route) => route.billingPlanRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'vehicles',
    loadChildren: () => import('../routes/vehicle.routes').then((route) => route.vehicleRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'clients',
    loadChildren: () => import('../routes/client.routes').then((route) => route.clientRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'drivers',
    loadChildren: () => import('../routes/driver.routes').then((route) => route.driverRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'extras',
    loadChildren: () => import('../routes/extra.routes').then((route) => route.extraRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'rentals',
    loadChildren: () => import('../routes/rental.routes').then((route) => route.rentalRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'configuration',
    loadChildren: () =>
      import('../routes/configuration.routes').then((route) => route.configurationRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'partners',
    loadChildren: () => import('../routes/partner.routes').then((route) => route.partnerRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'coupons',
    loadChildren: () => import('../routes/coupon.routes').then((route) => route.couponRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'profile',
    loadChildren: () => import('../routes/profile.routes').then((route) => route.profileRoutes),
    canMatch: [authenticatedUserGuard],
  },
  {
    path: 'admin',
    loadChildren: () => import('../routes/admin.routes').then((route) => route.adminRoutes),
    canMatch: [platformAdminOnlyGuard],
  },
  {
    path: '**',
    redirectTo: 'home',
  },
];
