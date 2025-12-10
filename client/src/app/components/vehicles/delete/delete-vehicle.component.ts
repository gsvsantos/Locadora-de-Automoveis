import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { Vehicle } from '../../../models/vehicles.models';
import { NotificationService } from '../../../services/notification.service';
import { VehicleService } from '../../../services/vehicle.service';
import { AsyncPipe, TitleCasePipe } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-vehicle.component',
  imports: [AsyncPipe, TitleCasePipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './delete-vehicle.component.html',
  styleUrl: './delete-vehicle.component.scss',
})
export class DeleteVehicleComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly vehicleService = inject(VehicleService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly vehicle$ = this.route.data.pipe(
    filter((data) => data['vehicle'] as boolean),
    map((data) => data['vehicle'] as Vehicle),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/vehicles']),
    };

    this.vehicle$
      .pipe(
        take(1),
        switchMap((vehicle) => this.vehicleService.delete(vehicle.id)),
      )
      .subscribe(deleteObserver);
  }
}
