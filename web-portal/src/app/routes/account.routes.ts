import { Routes } from '@angular/router';
import { DetailsAccountComponent } from '../components/account/details/details-account.component';
import { EditAccountComponent } from '../components/account/edit/edit-account.component';

export const accountRoutes: Routes = [
  {
    path: '',
    component: DetailsAccountComponent,
  },
  {
    path: 'edit',
    component: EditAccountComponent,
  },
];
