import { AsyncPipe, CurrencyPipe, DatePipe, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { Rental } from '../../../models/rental.models';

@Component({
  selector: 'app-list-rentals.component',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    DatePipe,
    TitleCasePipe,
    TranslocoModule,
    RouterLink,
    GsButtons,
  ],
  templateUrl: './list-rentals.component.html',
  styleUrl: './list-rentals.component.scss',
})
export class ListRentalsComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly showingInactive$ = this.route.queryParams.pipe(
    map((params) => ({
      isInactive: params['isActive'] === 'false',
    })),
  );

  protected readonly rentals$ = this.route.data.pipe(
    filter((data) => data['rentals'] as boolean),
    map((data) => data['rentals'] as Rental[]),
  );

  public toggleFilter(filter: boolean): void {
    const newFilter = filter;

    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { isActive: newFilter },
      queryParamsHandling: 'merge',
    });
  }
}
