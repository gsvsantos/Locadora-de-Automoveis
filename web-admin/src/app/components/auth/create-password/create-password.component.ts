import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { PartialObserver } from 'rxjs';
import { CreatePasswordRequestDto } from '../../../models/auth.models';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { newPasswordMatchValidator } from '../../../validators/auth.validators';

@Component({
  selector: 'app-create-password.component',
  imports: [ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-password.component.html',
  styleUrl: './create-password.component.scss',
})
export class CreatePasswordComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly router = inject(Router);
  protected readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected formGroup: FormGroup = this.formBuilder.group(
    {
      newPassword: ['', [Validators.required.bind(this), Validators.minLength(6)]],
      confirmNewPassword: ['', [Validators.required.bind(this), Validators.minLength(6)]],
    },
    { validators: newPasswordMatchValidator },
  );

  public get newPassword(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('newPassword');
  }

  public get confirmNewPassword(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('confirmNewPassword');
  }

  public createPassword(): void {
    if (this.formGroup.invalid) return;

    const changePasswordModel: CreatePasswordRequestDto = this.formGroup
      .value as CreatePasswordRequestDto;

    const changePasswordObserver: PartialObserver<void> = {
      error: (err: string) => this.notificationService.error(err),
      complete: () => (
        this.notificationService.success('Password Changed! Please, sign in again.'),
        void this.router.navigate(['/auth', 'login'])
      ),
    };

    this.authService.createPassword(changePasswordModel).subscribe(changePasswordObserver);
  }
}
