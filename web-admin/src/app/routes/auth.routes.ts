import { Routes } from '@angular/router';
import { unknownUserGuard } from '../guards/unknown-user.guard';
import { LoginComponent } from '../components/auth/login/login.component';
import { RegisterComponent } from '../components/auth/register/register.component';
import { ResetPasswordComponent } from '../components/auth/reset-password/reset-password.component';
import { ForgotPasswordComponent } from '../components/auth/forgot-password/forgot-password.component';

export const authRoutes: Routes = [
  {
    path: '',
    children: [
      { path: 'login', component: LoginComponent, canMatch: [unknownUserGuard] },
      { path: 'register', component: RegisterComponent, canMatch: [unknownUserGuard] },
      {
        path: 'forget-password',
        component: ForgotPasswordComponent,
        canMatch: [unknownUserGuard],
      },
      { path: 'reset-password', component: ResetPasswordComponent },
    ],
  },
];
