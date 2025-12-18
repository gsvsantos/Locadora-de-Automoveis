import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { PartialObserver } from 'rxjs';
import { ForgotPasswordRequestDto } from '../../../models/auth.models';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-forget-password.component',
  imports: [RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly router = inject(Router);
  protected readonly route = inject(ActivatedRoute);
  protected readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected formGroup: FormGroup = this.formBuilder.group({
    email: ['', [Validators.required.bind(this), Validators.email.bind(this)]],
  });

  public get email(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('email');
  }

  public forgotPassword(): void {
    if (this.formGroup.invalid) return;

    const forgotPasswordModel: ForgotPasswordRequestDto = this.formGroup
      .value as ForgotPasswordRequestDto;

    const forgetPasswordObserver: PartialObserver<void> = {
      error: (err: string) => (console.log(err), this.notificationService.error(err)),
      complete: () => (
        this.notificationService.success('Success! Please, check in your email.'),
        void this.router.navigate(['/auth', 'login'])
      ),
    };

    this.authService.forgotPassword(forgotPasswordModel).subscribe(forgetPasswordObserver);
  }
}
