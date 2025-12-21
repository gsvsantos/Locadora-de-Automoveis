import { Routes } from '@angular/router';
import { EditProfileComponent } from '../components/profile/edit/edit-profile.component';

export const profileRoutes: Routes = [
  {
    path: '',
    redirectTo: 'edit',
    pathMatch: 'full',
  },
  {
    path: 'edit',
    component: EditProfileComponent,
  },
];
