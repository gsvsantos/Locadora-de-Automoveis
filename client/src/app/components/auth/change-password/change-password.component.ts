import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { ChangePasswordRequestDto } from '../../../models/auth.models';
import { TranslocoModule } from '@jsverse/transloco';
import { PartialObserver } from 'rxjs';
import { newPasswordMatchValidator } from '../../../validators/auth.validators';

@Component({
  selector: 'app-change-password',
  imports: [ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss',
})
export class ChangePasswordComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly router = inject(Router);
  protected readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected formGroup: FormGroup = this.formBuilder.group(
    {
      currentPassword: ['', [Validators.required.bind(this), Validators.minLength(6)]],
      newPassword: ['', [Validators.required.bind(this), Validators.minLength(6)]],
      confirmNewPassword: ['', [Validators.required.bind(this), Validators.minLength(6)]],
    },
    { validators: newPasswordMatchValidator },
  );

  public get currentPassword(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('currentPassword');
  }

  public get newPassword(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('newPassword');
  }

  public get confirmNewPassword(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('confirmNewPassword');
  }

  public updatePassword(): void {
    if (this.formGroup.invalid) return;

    const changePasswordModel: ChangePasswordRequestDto = this.formGroup
      .value as ChangePasswordRequestDto;

    const changePasswordObserver: PartialObserver<void> = {
      error: (err: string) => this.notificationService.error(err),
      complete: () => (
        this.notificationService.success('Password Changed! Please, sign in again.'),
        void this.router.navigate(['/auth', 'login'])
      ),
    };

    this.authService.changePassword(changePasswordModel).subscribe(changePasswordObserver);
  }
}
