import { Component, inject, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PartialObserver } from 'rxjs';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { AuthApiResponse, RegisterAuthDto } from '../../../models/auth.models';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { passwordMatchValidator } from '../../../validators/auth.validators';
import { RecaptchaComponent, RecaptchaModule } from 'ng-recaptcha-2';
import { environment } from '../../../../environments/environment';
import { LocalStorageService, ThemeType } from '../../../services/local-storage.service';
import { NgxMaskDirective } from 'ngx-mask';

@Component({
  selector: 'app-register.component',
  imports: [
    RouterLink,
    NgxMaskDirective,
    ReactiveFormsModule,
    RecaptchaModule,
    TranslocoModule,
    GsButtons,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly router = inject(Router);
  protected readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;
  protected readonly recaptchaSiteKey: string = environment.captcha_key;

  @ViewChild('captchaRef') protected captchaElem!: RecaptchaComponent;

  private localStorageService = inject(LocalStorageService);
  protected themeValue: ThemeType = this.localStorageService.getCurrentTheme();

  protected formGroup: FormGroup = this.formBuilder.group(
    {
      userName: ['', [Validators.required.bind(this), Validators.minLength(5)]],
      fullName: [
        '',
        [
          Validators.required.bind(this),
          Validators.minLength(3),
          Validators.pattern(/^[a-zA-ZÄäÖöÜüÀàÈèÌìÒòÙùÁáÉéÍíÓóÚúÝýÂâÊêÎîÔôÛûÃãÑñÇç'\-\s]+$/),
        ],
      ],
      email: ['', [Validators.required.bind(this), Validators.email.bind(this)]],
      phoneNumber: [
        '',
        [
          Validators.required.bind(this),
          Validators.pattern(/^\(?\d{2}\)?\s?(9\d{4}|\d{4})-?\d{4}$/).bind(this),
        ],
      ],
      password: ['', [Validators.required.bind(this), Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required.bind(this), Validators.minLength(6)]],
      recaptchaToken: ['', Validators.required.bind(this)],
    },
    { validators: passwordMatchValidator },
  );

  public get userName(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('userName');
  }

  public get fullName(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('fullName');
  }

  public get email(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('email');
  }

  public get phoneNumber(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('phoneNumber');
  }

  public get password(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('password');
  }

  public get confirmPassword(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('confirmPassword');
  }

  public get recaptchaToken(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('recaptchaToken');
  }

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: RegisterAuthDto = this.formGroup.value as RegisterAuthDto;

    const registerObserver: PartialObserver<AuthApiResponse> = {
      error: (err: string) => (this.notificationService.error(err), this.captchaElem.reset()),
      complete: () => void this.router.navigate(['/home']),
    };

    this.authService.register(registerModel).subscribe(registerObserver);
  }

  public googleAuth(): void {
    this.authService.loginWithGoogle();
  }

  public onCaptchaResolved(captchaToken: string | null): void {
    if (!captchaToken) return;
    this.formGroup.patchValue({ recaptchaToken: captchaToken });
  }
}
