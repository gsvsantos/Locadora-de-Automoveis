import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { CouponService } from '../../../services/coupon.service';
import { CouponDto } from '../../../models/coupon.models';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observer } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { NotificationService } from '../../../services/notification.service';
import { TranslocoModule } from '@jsverse/transloco';
import { ListPartnerDto } from '../../../models/partner.models';

@Component({
  selector: 'app-create-coupon.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-coupon.component.html',
  styleUrl: './create-coupon.component.scss',
})
export class CreateCouponComponent {
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

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: CouponDto = this.formGroup.value as CouponDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(`Coupon "${registerModel.name}" registered successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/coupons']),
    };

    this.couponService.register(registerModel).subscribe(registerObserver);
  }
}
