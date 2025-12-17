import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, switchMap, take } from 'rxjs';
import { Coupon } from '../../../models/coupon.models';
import { CouponService } from '../../../services/coupon.service';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-coupon.component',
  imports: [AsyncPipe, CurrencyPipe, DatePipe, RouterLink, TranslocoModule, GsButtons],
  templateUrl: './delete-coupon.component.html',
  styleUrl: './delete-coupon.component.scss',
})
export class DeleteCouponComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly couponService = inject(CouponService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly coupon$ = this.route.data.pipe(
    filter((data) => data['coupon'] as boolean),
    map((data) => data['coupon'] as Coupon),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/coupons']),
    };

    this.coupon$
      .pipe(
        take(1),
        switchMap((coupon) => this.couponService.delete(coupon.id)),
      )
      .subscribe(deleteObserver);
  }
}
