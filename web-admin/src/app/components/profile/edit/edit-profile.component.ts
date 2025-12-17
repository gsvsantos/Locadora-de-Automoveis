import { Component } from '@angular/core';
import { TranslocoModule } from '@jsverse/transloco';
import { ChangePasswordComponent } from '../change-password/change-password.component';
import { ProfileUpdateComponent } from '../update/profile-update.component';

@Component({
  selector: 'app-edit-profile.component',
  imports: [TranslocoModule, ChangePasswordComponent, ProfileUpdateComponent],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.scss',
})
export class EditProfileComponent {}
