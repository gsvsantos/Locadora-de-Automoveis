import { AsyncPipe, CurrencyPipe, DatePipe, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { Coupon } from '../../../models/coupon.models';

@Component({
  selector: 'app-list-coupons.component',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    DatePipe,
    TitleCasePipe,
    TranslocoModule,
    RouterLink,
    GsButtons,
  ],
  templateUrl: './list-coupons.component.html',
  styleUrl: './list-coupons.component.scss',
})
export class ListCouponsComponent {
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

  protected readonly coupons$ = this.route.data.pipe(
    filter((data) => data['coupons'] as boolean),
    map((data) => data['coupons'] as Coupon[]),
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
