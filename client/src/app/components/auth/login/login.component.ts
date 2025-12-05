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

@Component({
  selector: 'app-login.component',
  imports: [RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
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

  protected formGroup: FormGroup = this.formBuilder.group({
    userName: ['', [Validators.required.bind(this), Validators.minLength(3)]],
    password: ['', [Validators.required.bind(this), Validators.minLength(6)]],
  });

  public get userName(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('userName');
  }

  public get password(): AbstractControl<unknown, unknown, unknown> | null {
    return this.formGroup.get('password');
  }

  public login(): void {
    if (this.formGroup.invalid) return;

    const loginModel: LoginAuthDto = this.formGroup.value as LoginAuthDto;
    console.log(loginModel);
    const loginObserver: PartialObserver<AuthApiResponse> = {
      error: (err: string) => (console.log(err), this.notificationService.error(err)),
      complete: () => void this.router.navigate(['/home']),
    };

    this.authService.login(loginModel).subscribe(loginObserver);
  }
}
