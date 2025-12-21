import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, tap, distinctUntilChanged, Observer, take } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { EmployeeDto } from '../../../models/employee.models';
import { AuthService } from '../../../services/auth.service';
import { EmployeeService } from '../../../services/employee.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-profile-update',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './profile-update.component.html',
  styleUrl: './profile-update.component.scss',
})
export class ProfileUpdateComponent {
  private readonly authService = inject(AuthService);
  protected isSubmitting = false;
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly employeeService = inject(EmployeeService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;
  protected readonly accessToken$ = this.authService.getAccessToken();

  public readonly user$ = this.accessToken$.pipe(
    map((accessToken) => accessToken?.user),
    tap((user) =>
      this.formGroup.patchValue({
        fullName: user?.fullName,
      }),
    ),
    distinctUntilChanged((first, second) => first?.fullName === second?.fullName),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    fullName: [
      '',
      [
        Validators.required.bind(this),
        Validators.minLength(3),
        Validators.pattern(/^[a-zA-ZÄäÖöÜüÀàÈèÌìÒòÙùÁáÉéÍíÓóÚúÝýÂâÊêÎîÔôÛûÃãÑñÇç'\-\s]+$/),
      ],
    ],
  });

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: EmployeeDto = this.formGroup.value as EmployeeDto;

    const updateObserver: Observer<IdApiResponse> = {
      next: () => {
        this.notificationService.success(
          `Employee "${updateModel.fullName}" updated successfully!`,
        );

        this.authService
          .refresh()
          .pipe(take(1))
          .subscribe({
            next: () => {
              void this.router.navigate(['/home']);
            },
          });
      },
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/home']),
    };

    this.employeeService.selfUpdate(updateModel).pipe(take(1)).subscribe(updateObserver);
  }
}
