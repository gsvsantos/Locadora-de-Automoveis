import { Component } from '@angular/core';
import { TranslocoModule } from '@jsverse/transloco';
import { ChangePasswordComponent } from '../../profile/change-password/change-password.component';
import { ProfileUpdateComponent } from '../../profile/update/profile-update.component';

@Component({
  selector: 'app-profile.component',
  imports: [TranslocoModule, ChangePasswordComponent, ProfileUpdateComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent {}
