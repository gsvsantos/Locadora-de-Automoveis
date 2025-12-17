import { Routes } from '@angular/router';
import { CreatePasswordComponent } from '../components/auth/create-password/create-password.component';
import { EditProfileComponent } from '../components/profile/edit/edit-profile.component';

export const profileRoutes: Routes = [
  {
    path: '',
    redirectTo: 'edit',
    pathMatch: 'full',
  },
  { path: 'create-password', component: CreatePasswordComponent },
  {
    path: 'edit',
    component: EditProfileComponent,
  },
];
