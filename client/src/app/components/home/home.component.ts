import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observable, shareReplay } from 'rxjs';
import { Client } from '../../models/client.models';
import { MostUsedCouponDto } from '../../models/coupon.models';
import { Rental } from '../../models/rental.models';
import { Vehicle } from '../../models/vehicles.models';
import { differenceInDays } from 'date-fns';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe, RouterLink, TranslocoModule, GsButtons],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly clients$ = this.route.data.pipe(
    filter((data) => data['clients'] as boolean),
    map((data) => data['clients'] as Client[]),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly coupons$ = this.route.data.pipe(
    filter((data) => data['coupons'] as boolean),
    map((data) => data['coupons'] as MostUsedCouponDto[]),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly rentals$ = this.route.data.pipe(
    filter((data) => data['rentals'] as boolean),
    map((data) => data['rentals'] as Rental[]),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly vehicles$ = this.route.data.pipe(
    filter((data) => data['vehicles'] as boolean),
    map((data) => data['vehicles'] as Vehicle[]),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly totalVehicles$: Observable<number> = this.vehicles$.pipe(
    map((vehicles) => vehicles.length),
  );

  protected readonly activeRentals$: Observable<number> = this.rentals$.pipe(
    map((rentals) => rentals.filter((rental) => rental.returnDate === null).length),
  );

  protected readonly activeClients$: Observable<number> = this.clients$.pipe(
    map((clients) => clients.filter((client) => client.isActive === true).length),
  );

  protected readonly pendingReturns$: Observable<Rental[]> = this.rentals$.pipe(
    map((rentals) => {
      const today = new Date();
      const futureLimit = 7;

      return rentals.filter((rental) => {
        const isRentalActive = rental.returnDate === null;

        if (!isRentalActive || !rental.expectedReturnDate) {
          return false;
        }

        const expectedDate = new Date(rental.expectedReturnDate);
        const daysUntilReturn = differenceInDays(expectedDate, today);

        return daysUntilReturn >= 0 && daysUntilReturn <= futureLimit;
      });
    }),
  );

  protected readonly topCoupons$: Observable<MostUsedCouponDto[]> = this.coupons$;
}
