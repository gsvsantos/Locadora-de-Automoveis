import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PartialObserver } from 'rxjs';
import { AuthApiResponse, LoginAuthDto } from '../../../models/auth.models';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { RecaptchaModule } from 'ng-recaptcha-2';
import { environment } from '../../../../environments/environment';
import { LocalStorageService, ThemeType } from '../../../services/local-storage.service';

@Component({
  selector: 'app-login.component',
  imports: [RouterLink, ReactiveFormsModule, RecaptchaModule, TranslocoModule, GsButtons],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly router = inject(Router);
  protected readonly authService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;
  protected readonly recaptchaSiteKey: string = environment.captcha_key;

  private localStorageService = inject(LocalStorageService);
  protected themeValue: ThemeType = this.localStorageService.getCurrentTheme();

  protected formGroup: FormGroup = this.formBuilder.group({
    userName: ['', [Validators.required.bind(this), Validators.minLength(3)]],
    password: ['', [Validators.required.bind(this), Validators.minLength(6)]],
    recaptchaToken: ['', Validators.required.bind(this)],
  });

  public get userName(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('userName');
  }

  public get password(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('password');
  }

  public get recaptchaToken(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('recaptchaToken');
  }

  public login(): void {
    if (this.formGroup.invalid) return;

    const loginModel: LoginAuthDto = this.formGroup.value as LoginAuthDto;

    const loginObserver: PartialObserver<AuthApiResponse> = {
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/home']),
    };

    this.authService.login(loginModel).subscribe(loginObserver);
  }

  public googleAuth(): void {
    this.authService.loginWithGoogle();
  }

  public onCaptchaResolved(captchaToken: string | null): void {
    if (!captchaToken) return;
    this.formGroup.patchValue({ recaptchaToken: captchaToken });
  }
}
