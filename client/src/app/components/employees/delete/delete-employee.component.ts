import { Component, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { EmployeeService } from '../../../services/employee.service';
import { NotificationService } from '../../../services/notification.service';
import { TranslocoModule } from '@jsverse/transloco';
import { filter, map, Observer, shareReplay, switchMap, take } from 'rxjs';
import { Employee } from '../../../models/employee.models';

@Component({
  selector: 'app-delete-employee.component',
  imports: [RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './delete-employee.component.html',
  styleUrl: './delete-employee.component.scss',
})
export class DeleteEmployeeComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly employeeService = inject(EmployeeService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly employee$ = this.route.data.pipe(
    filter((data) => data['employee'] as boolean),
    map((data) => data['employee'] as Employee),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/employees']),
    };

    this.employee$
      .pipe(
        take(1),
        switchMap((employee) => this.employeeService.delete(employee.id)),
      )
      .subscribe(deleteObserver);
  }
}
