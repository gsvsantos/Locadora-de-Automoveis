import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observer, take, switchMap, shareReplay, tap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { Client } from '../../../models/client.models';
import { Driver } from '../../../models/driver.models';
import { Employee } from '../../../models/employee.models';
import { Extra } from '../../../models/extra.models';
import { Vehicle } from '../../../models/vehicles.models';
import { NotificationService } from '../../../services/notification.service';
import { RentalService } from '../../../services/rental.service';
import { CreateRentalDto, RentalDetailsDto } from '../../../models/rental.models';
import { dateToInputDateString } from '../../../utils/date.utils';
import { Coupon } from '../../../models/coupon.models';
import { needsIndividualValidator } from '../../../validators/driver.validators';

@Component({
  selector: 'app-update-rental.component',
  imports: [AsyncPipe, CurrencyPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-rental.component.html',
  styleUrl: './update-rental.component.scss',
})
export class UpdateRentalComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly rentalService = inject(RentalService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly clients$ = this.route.data.pipe(
    filter((data) => data['clients'] as boolean),
    map((data) => data['clients'] as Client[]),
  );

  protected readonly coupons$ = this.route.data.pipe(
    filter((data) => data['coupons'] as boolean),
    map((data) => data['coupons'] as Coupon[]),
  );

  protected readonly employees$ = this.route.data.pipe(
    filter((data) => data['employees'] as boolean),
    map((data) => data['employees'] as Employee[]),
  );

  protected readonly drivers$ = this.route.data.pipe(
    filter((data) => data['drivers'] as boolean),
    map((data) => data['drivers'] as Driver[]),
  );

  protected readonly vehicles$ = this.route.data.pipe(
    filter((data) => data['vehicles'] as boolean),
    map((data) => data['vehicles'] as Vehicle[]),
  );

  protected readonly extras$ = this.route.data.pipe(
    filter((data) => data['extras'] as boolean),
    map((data) => data['extras'] as Extra[]),
  );

  protected readonly rental$ = this.route.data.pipe(
    filter((data) => data['rental'] as boolean),
    map((data) => data['rental'] as RentalDetailsDto),
    tap((rental: RentalDetailsDto) => {
      this.fillDriverFields(rental);
      console.log(this.formGroup);
    }),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group(
    {
      startDate: [null, [Validators.required.bind(this)]],
      expectedReturnDate: [null, [Validators.required.bind(this)]],
      startKm: [null, [Validators.required.bind(this)]],
      clientId: [null, [Validators.required.bind(this)]],
      employeeId: [null, [Validators.required.bind(this)]],
      driverId: [null, [Validators.required.bind(this)]],
      vehicleId: [null, [Validators.required.bind(this)]],
      couponId: [null],
      billingPlanType: [null, [Validators.required.bind(this)]],
      estimatedKilometers: [null],
      rentalRentalExtrasIds: [[]],
    },
    { validators: [needsIndividualValidator] },
  );

  public get startDate(): AbstractControl | null {
    return this.formGroup.get('startDate');
  }

  public get expectedReturnDate(): AbstractControl | null {
    return this.formGroup.get('expectedReturnDate');
  }

  public get startKm(): AbstractControl | null {
    return this.formGroup.get('startKm');
  }

  public get clientId(): AbstractControl | null {
    return this.formGroup.get('clientId');
  }

  public get employeeId(): AbstractControl | null {
    return this.formGroup.get('employeeId');
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

  public get billingPlanType(): AbstractControl | null {
    return this.formGroup.get('billingPlanType');
  }

  public get estimatedKilometers(): AbstractControl | null {
    return this.formGroup.get('estimatedKilometers');
  }

  public get rentalRentalExtrasIds(): AbstractControl | null {
    return this.formGroup.get('rentalRentalExtrasIds');
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    this.startDate?.enable();
    const updateModel: CreateRentalDto = this.formGroup.value as CreateRentalDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Rental updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/rentals']),
    };

    this.rental$
      .pipe(
        take(1),
        switchMap((vehicle) => this.rentalService.update(vehicle.id, updateModel)),
      )
      .subscribe(updateObserve);
  }

  private fillDriverFields(rental: RentalDetailsDto): void {
    this.formGroup.patchValue({
      startDate: dateToInputDateString(rental.startDate),
      expectedReturnDate: dateToInputDateString(rental.expectedReturnDate),
      startKm: rental.startKm,
      billingPlanType: rental.billingPlanType,
      estimatedKilometers: rental.estimatedKilometers,
      clientId: rental.client.id,
      employeeId: rental.employee.id,
      driverId: rental.driver.id,
      vehicleId: rental.vehicle.id,
      rentalRentalExtrasIds: rental.rentalExtras
        ? rental.rentalExtras.map((extra) => extra.id)
        : [],
    });

    this.startDate?.disable();
  }
}
