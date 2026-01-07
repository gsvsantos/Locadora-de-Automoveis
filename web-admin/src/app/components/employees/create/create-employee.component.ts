import { EmployeeService } from '../../../services/employee.service';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observer } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { NotificationService } from '../../../services/notification.service';
import { EmployeeDto } from '../../../models/employee.models';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { TranslocoModule } from '@jsverse/transloco';
import { NgxMaskDirective } from 'ngx-mask';

@Component({
  selector: 'app-create-employee.component',
  imports: [RouterLink, NgxMaskDirective, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-employee.component.html',
  styleUrl: './create-employee.component.scss',
})
export class CreateEmployeeComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly employeeService = inject(EmployeeService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected formGroup: FormGroup = this.formBuilder.group({
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
    userName: ['', [Validators.required.bind(this), Validators.minLength(5)]],
    password: ['', [Validators.required.bind(this), Validators.minLength(6)]],
    admissionDate: ['', [Validators.required.bind(this)]],
    salary: [null, [Validators.required.bind(this), Validators.min(100)]],
  });

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
  }
  public get email(): AbstractControl | null {
    return this.formGroup.get('email');
  }

  public get phoneNumber(): AbstractControl | null {
    return this.formGroup.get('phoneNumber');
  }

  public get userName(): AbstractControl | null {
    return this.formGroup.get('userName');
  }

  public get password(): AbstractControl | null {
    return this.formGroup.get('password');
  }

  public get admissionDate(): AbstractControl | null {
    return this.formGroup.get('admissionDate');
  }

  public get salary(): AbstractControl | null {
    return this.formGroup.get('salary');
  }

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: EmployeeDto = this.formGroup.value as EmployeeDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(
          `Employee "${registerModel.fullName}" registered successfully!`,
        ),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/employees']),
    };

    this.employeeService.register(registerModel).subscribe(registerObserver);
  }
}
