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
import { filter, map, Observable, defer, of, startWith, tap, Observer } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { CreateSelfRentalDto } from '../../../models/rental.models';
import { Vehicle } from '../../../models/vehicle.models';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';
import { Coupon } from '../../../models/coupon.models';
import { Extra } from '../../../models/extra.models';
import { RentalService } from '../../../services/rental.service';
import { Driver } from '../../../models/driver.models';

@Component({
  selector: 'app-create-self.component',
  imports: [AsyncPipe, CurrencyPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-self.component.html',
  styleUrl: './create-self.component.scss',
})
export class CreateSelfComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly rentalService = inject(RentalService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly coupons$ = this.route.data.pipe(
    filter((data) => data['coupons'] as boolean),
    map((data) => data['coupons'] as Coupon[]),
  );

  protected readonly drivers$ = this.route.data.pipe(
    filter((data) => data['drivers'] as boolean),
    map((data) => data['drivers'] as Driver[]),
  );

  protected readonly extras$ = this.route.data.pipe(
    filter((data) => data['extras'] as boolean),
    map((data) => data['extras'] as Extra[]),
  );

  protected readonly vehicle$ = this.route.data.pipe(
    filter((data) => data['vehicle'] as boolean),
    map((data) => data['vehicle'] as Vehicle),
    tap((vehicle) => this.fillVehicleFields(vehicle)),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    startDate: [new Date().toISOString().substring(0, 10), [Validators.required.bind(this)]],
    expectedReturnDate: [null, [Validators.required.bind(this)]],
    driverId: [null, [Validators.required.bind(this)]],
    vehicleId: [null, [Validators.required.bind(this)]],
    couponId: [null],
    billingPlanType: [null, [Validators.required.bind(this)]],
    estimatedKilometers: [null],
    rentalRentalExtrasIds: [[]],
  });

  protected readonly hasCoupon$: Observable<boolean> = this.coupons$.pipe(
    map((res) => res.length >= 1),
  );

  protected readonly hasExtras$: Observable<boolean> = this.extras$.pipe(
    map((res) => res.length >= 1),
  );

  protected readonly billingPlanLogic$: Observable<string | null> = defer(() => {
    const planControl = this.billingPlanType;

    if (!planControl) return of(null);

    return planControl.valueChanges.pipe(
      startWith(planControl.value),
      tap((planType: string) => {
        const kmControl = this.estimatedKilometers;
        if (!kmControl) return;

        if (planType === 'Controlled') {
          kmControl.setValidators([Validators.required.bind(this), Validators.min(1)]);
        } else {
          kmControl.clearValidators();
          kmControl.setValue(null);
        }
        kmControl.updateValueAndValidity();
      }),
    );
  });

  public get startDate(): AbstractControl | null {
    return this.formGroup.get('startDate');
  }

  public get expectedReturnDate(): AbstractControl | null {
    return this.formGroup.get('expectedReturnDate');
  }

  public get driverId(): AbstractControl | null {
    return this.formGroup.get('driverId');
  }

  public get vehicleId(): AbstractControl | null {
    return this.formGroup.get('vehicleId');
  }

  public get couponId(): AbstractControl | null {
    return this.formGroup.get('couponId');
  }

  public get billingPlanType(): AbstractControl<string> | null {
    return this.formGroup.get('billingPlanType');
  }

  public get estimatedKilometers(): AbstractControl | null {
    return this.formGroup.get('estimatedKilometers');
  }

  public get rentalRentalExtrasIds(): AbstractControl | null {
    return this.formGroup.get('rentalRentalExtrasIds');
  }

  public register(): void {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    const registerModel: CreateSelfRentalDto = this.formGroup.getRawValue() as CreateSelfRentalDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Rental requested successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/my-rentals']),
    };

    this.rentalService.createSelfRental(registerModel).subscribe(registerObserver);
  }

  private fillVehicleFields(vehicle: Vehicle): void {
    this.formGroup.patchValue({
      vehicleId: vehicle.id,
    });
  }
}
