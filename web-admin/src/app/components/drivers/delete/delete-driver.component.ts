import { AsyncPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { NotificationService } from '../../../services/notification.service';
import { Driver } from './../../../models/driver.models';
import { DriverService } from './../../../services/driver.service';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-delete-driver.component',
  imports: [AsyncPipe, DatePipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './delete-driver.component.html',
  styleUrl: './delete-driver.component.scss',
})
export class DeleteDriverComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly driverService = inject(DriverService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly driver$ = this.route.data.pipe(
    filter((data) => data['driver'] as boolean),
    map((data) => data['driver'] as Driver),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/drivers']),
    };

    this.driver$
      .pipe(
        take(1),
        switchMap((driver) => this.driverService.delete(driver.id)),
      )
      .subscribe(deleteObserver);
  }
}
