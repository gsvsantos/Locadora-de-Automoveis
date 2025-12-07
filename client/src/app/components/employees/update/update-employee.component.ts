import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observer, shareReplay, switchMap, take, tap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { Employee, EmployeeDto } from '../../../models/employee.models';
import { EmployeeService } from '../../../services/employee.service';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';
import { dateToInputDateString } from '../../../utils/date.utils';

@Component({
  selector: 'app-update-employee.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-employee.component.html',
  styleUrl: './update-employee.component.scss',
})
export class UpdateEmployeeComponent {
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
    admissionDate: ['', [Validators.required.bind(this)]],
    salary: [0, [Validators.required.bind(this), Validators.min(100)]],
  });


  protected readonly employee$ = this.route.data.pipe(
    filter((data) => data['employee'] as boolean),
    map((data) => data['employee'] as Employee),
    tap((employee: Employee) => {
      this.formGroup.patchValue({
        fullName: employee.fullName,
        admissionDate: dateToInputDateString(employee.admissionDate),
        salary: employee.salary,
      });
    }),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
  }

  public get admissionDate(): AbstractControl | null {
    return this.formGroup.get('admissionDate');
  }

  public get salary(): AbstractControl | null {
    return this.formGroup.get('salary');
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: EmployeeDto = this.formGroup.value as EmployeeDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(
          `Employee "${updateModel.fullName}" updated successfully!`,
        ),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/employees']),
    };

    this.employee$
      .pipe(
        take(1),
        switchMap((employee) => this.employeeService.update(employee.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
