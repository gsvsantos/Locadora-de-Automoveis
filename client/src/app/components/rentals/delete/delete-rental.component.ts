import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { RentalDetailsDto } from '../../../models/rental.models';
import { NotificationService } from '../../../services/notification.service';
import { RentalService } from '../../../services/rental.service';
import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-rental.component',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    DatePipe,
    RouterLink,
    ReactiveFormsModule,
    TranslocoModule,
    GsButtons,
  ],
  templateUrl: './delete-rental.component.html',
  styleUrl: './delete-rental.component.scss',
})
export class DeleteRentalComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly rentalService = inject(RentalService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly rental$ = this.route.data.pipe(
    filter((data) => data['rental'] as boolean),
    map((data) => data['rental'] as RentalDetailsDto),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/rentals']),
    };

    this.rental$
      .pipe(
        take(1),
        switchMap((rental) => this.rentalService.delete(rental.id)),
      )
      .subscribe(deleteObserver);
  }
}
