import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay } from 'rxjs';
import { RentalDetailsDto, RentalReturn } from '../../../models/rental.models';
import { AsyncPipe, DatePipe, CurrencyPipe, TitleCasePipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-rental-details.component',
  imports: [
    AsyncPipe,
    DatePipe,
    CurrencyPipe,
    TitleCasePipe,
    RouterLink,
    TranslocoModule,
    GsButtons,
  ],
  templateUrl: './rental-details.component.html',
  styleUrl: './rental-details.component.scss',
})
export class RentalDetailsComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly rental$ = this.route.data.pipe(
    filter((data) => !!data['rental']),
    map((data) => data['rental'] as RentalDetailsDto),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly rentalReturn$ = this.rental$.pipe(
    map((rental) => (rental.rentalReturn as RentalReturn | null) ?? null),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly isReturned$ = this.rental$.pipe(
    map((rental) => !!rental.returnDate || !!rental.rentalReturn),
    shareReplay({ bufferSize: 1, refCount: true }),
  );
}
