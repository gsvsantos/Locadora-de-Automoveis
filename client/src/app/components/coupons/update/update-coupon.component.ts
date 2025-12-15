import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, tap, shareReplay, Observer, take, switchMap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { Coupon, CouponDto } from '../../../models/coupon.models';
import { ListPartnerDto } from '../../../models/partner.models';
import { CouponService } from '../../../services/coupon.service';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-update-coupon.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-coupon.component.html',
  styleUrl: './update-coupon.component.scss',
})
export class UpdateCouponComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly couponService = inject(CouponService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly partners$ = this.route.data.pipe(
    filter((data) => data['partners'] as boolean),
    map((data) => data['partners'] as ListPartnerDto[]),
  );

  protected readonly coupon$ = this.route.data.pipe(
    filter((data) => data['coupon'] as boolean),
    map((data) => data['coupon'] as Coupon),
    tap((coupon: Coupon) => this.formGroup.patchValue(coupon)),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    name: ['', [Validators.required.bind(this), Validators.minLength(4)]],
    discountValue: ['', [Validators.required.bind(this), Validators.min(1)]],
    expirationDate: ['', [Validators.required.bind(this)]],
    partnerId: ['', [Validators.required.bind(this)]],
  });

  public get name(): AbstractControl | null {
    return this.formGroup.get('name');
  }

  public get discountValue(): AbstractControl | null {
    return this.formGroup.get('discountValue');
  }

  public get expirationDate(): AbstractControl | null {
    return this.formGroup.get('expirationDate');
  }

  public get partnerId(): AbstractControl | null {
    return this.formGroup.get('partnerId');
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: CouponDto = this.formGroup.value as CouponDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(`Group "${updateModel.name}" updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/coupons']),
    };

    this.coupon$
      .pipe(
        take(1),
        switchMap((coupon) => this.couponService.update(coupon.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
