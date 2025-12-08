import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observer } from 'rxjs';
import { Client } from '../../../models/client.models';
import { NotificationService } from '../../../services/notification.service';
import { RentalService } from './../../../services/rental.service';
import { Component, inject } from '@angular/core';
import { Employee } from '../../../models/employee.models';
import { Driver } from '../../../models/driver.models';
import { Vehicle } from '../../../models/vehicles.models';
import { needsIndividual } from '../../../validators/driver.validators';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';
import { Extra } from '../../../models/extra.models';
import { CreateRentalDto } from '../../../models/rental.models';
import { IdApiResponse } from '../../../models/api.models';

@Component({
  selector: 'app-create-rental.component',
  imports: [AsyncPipe, CurrencyPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-rental.component.html',
  styleUrl: './create-rental.component.scss',
})
export class CreateRentalComponent {
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

  // protected readonly coupons$ = this.route.data.pipe(
  //   filter((data) => data['coupons'] as boolean),
  //   map((data) => data['coupons'] as Coupon[]),
  // );

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

  protected formGroup: FormGroup = this.formBuilder.group(
    {
      startDate: [null, [Validators.required.bind(this)]],
      expectedReturnDate: [null, [Validators.required.bind(this)]],
      startKm: [null, [Validators.required.bind(this)]],
      clientId: [null, [Validators.required.bind(this)]],
      employeeId: [null, [Validators.required.bind(this)]],
      driverId: [null, [Validators.required.bind(this)]],
      vehicleId: [null, [Validators.required.bind(this)]],
      // couponId: [null, [Validators.required.bind(this)]],
      billingPlanType: ['', [Validators.required.bind(this)]],
      estimatedKilometers: [null],
      rentalRentalExtrasIds: [null],
    },
    { validators: [needsIndividual] },
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

  // public get couponId(): AbstractControl | null {
  //   return this.formGroup.get('couponId');
  // }

  public get billingPlanType(): AbstractControl | null {
    return this.formGroup.get('billingPlanType');
  }

  public get estimatedKilometers(): AbstractControl | null {
    return this.formGroup.get('estimatedKilometers');
  }

  public get rentalRentalExtrasIds(): AbstractControl | null {
    return this.formGroup.get('rentalRentalExtrasIds');
  }

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: CreateRentalDto = this.formGroup.getRawValue() as CreateRentalDto;

    console.log(registerModel);

    const registerObserver: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Rental registered successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/rentals']),
    };

    this.rentalService.register(registerModel).subscribe(registerObserver);
  }
}
