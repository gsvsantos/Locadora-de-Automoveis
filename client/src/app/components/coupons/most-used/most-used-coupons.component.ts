import { AsyncPipe, CurrencyPipe, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { MostUsedCouponDto } from '../../../models/coupon.models';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-most-used-coupons.component',
  imports: [AsyncPipe, CurrencyPipe, TitleCasePipe, TranslocoModule, RouterLink, GsButtons],
  templateUrl: './most-used-coupons.component.html',
  styleUrl: './most-used-coupons.component.scss',
})
export class MostUsedCouponsComponent {
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
    map((data) => data['coupons'] as MostUsedCouponDto[]),
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
