/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, PartialObserver } from 'rxjs';
import { ResetPasswordRequestDto } from '../../../models/auth.models';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { newPasswordMatchValidator } from '../../../validators/auth.validators';

@Component({
  selector: 'app-reset-password.component',
  imports: [AsyncPipe, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
})
export class ResetPasswordComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly router = inject(Router);
  protected readonly route = inject(ActivatedRoute);
  protected readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly routeParams$ = this.route.queryParams.pipe(
    map((params) => {
      const token = params['token'];
      const email = params['email'];

      if (!token || !email) {
        setTimeout(() => {
          this.notificationService.error('Invalid Link');
          void this.router.navigate(['/auth', 'login']);
        });
        return null;
      }

      return { token, email };
    }),
  );

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

  public resetPassword(email: string, token: string): void {
    if (this.formGroup.invalid) return;

    const resetPasswordModelPartia: ResetPasswordRequestDto = this.formGroup
      .value as ResetPasswordRequestDto;

    const resetPasswordModel: ResetPasswordRequestDto = {
      email: email,
      token: token,
      newPassword: resetPasswordModelPartia.newPassword,
      confirmNewPassword: resetPasswordModelPartia.confirmNewPassword,
    };
    console.log(resetPasswordModel);

    const resetPasswordObserver: PartialObserver<void> = {
      error: (err: string) => (console.log(err), this.notificationService.error(err)),
      complete: () => (
        this.notificationService.success('Success! Please, sign in again.'),
        void this.router.navigate(['/auth', 'login'])
      ),
    };

    this.authService.resetPassword(resetPasswordModel).subscribe(resetPasswordObserver);
  }
}
