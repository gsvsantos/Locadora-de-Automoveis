import { Routes } from '@angular/router';
import { unknownUserGuard } from '../guards/unknown-user.guard';
import { LoginComponent } from '../components/auth/login/login.component';
import { RegisterComponent } from '../components/auth/register/register.component';
import { ResetPasswordComponent } from '../components/auth/reset-password/reset-password.component';
import { ForgetPasswordComponent } from '../components/auth/forget-password/forget-password.component';

export const authRoutes: Routes = [
  {
    path: '',
    children: [
      { path: 'login', component: LoginComponent, canActivate: [unknownUserGuard] },
      { path: 'register', component: RegisterComponent, canActivate: [unknownUserGuard] },
      {
        path: 'forget-password',
        component: ForgetPasswordComponent,
        canActivate: [unknownUserGuard],
      },
      { path: 'reset-password', component: ResetPasswordComponent },
    ],
  },
];
