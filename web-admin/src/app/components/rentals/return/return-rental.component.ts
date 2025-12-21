/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { BillingPlanDto, ControlledBillingDto } from './../../../models/billing-plan.models';
import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  AbstractControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import {
  filter,
  map,
  shareReplay,
  Observer,
  take,
  switchMap,
  combineLatest,
  startWith,
  debounceTime,
  distinctUntilChanged,
} from 'rxjs';
import {
  FUEL_LEVEL_PERCENTAGE,
  RentalDetailsDto,
  RentalVehicleDto,
  RentalReturnDto,
} from '../../../models/rental.models';
import { NotificationService } from '../../../services/notification.service';
import { RentalService } from '../../../services/rental.service';
import { IdApiResponse } from '../../../models/api.models';
import { Extra } from '../../../models/extra.models';
import { Configuration } from '../../../models/configuration.models';
import { FuelType } from '../../../models/vehicles.models';

type fuelLevelAtReturnKey = keyof typeof FUEL_LEVEL_PERCENTAGE;

type ReturnFormValue = {
  endKm: number | null;
  fuelLevelAtReturn: fuelLevelAtReturnKey | null;
};

@Component({
  selector: 'app-return-rental.component',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    DatePipe,
    RouterLink,
    ReactiveFormsModule,
    TranslocoModule,
    GsButtons,
  ],
  templateUrl: './return-rental.component.html',
  styleUrl: './return-rental.component.scss',
})
export class ReturnRentalComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly rentalService = inject(RentalService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  private readonly returnDate: Date = new Date();

  protected readonly rental$ = this.route.data.pipe(
    filter((data) => data['rental'] as boolean),
    map((data) => data['rental'] as RentalDetailsDto),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly configuration$ = this.route.data.pipe(
    filter((data) => data['configuration'] as boolean),
    map((data) => data['configuration'] as Configuration),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    endKm: ['', [Validators.required.bind(this)]],
    fuelLevelAtReturn: ['', [Validators.required.bind(this)]],
  });

  protected readonly totalPrice$ = combineLatest([
    this.rental$,
    this.configuration$,
    this.formGroup.valueChanges.pipe(
      startWith(this.formGroup.value as ReturnFormValue),
      debounceTime(300),
      distinctUntilChanged(
        (first, second) =>
          first?.endKm === second?.endKm && first?.fuelLevelAtReturn === second?.fuelLevelAtReturn,
      ),
    ),
  ]).pipe(
    map(([rental, config, formValue]) => {
      if (this.formGroup.invalid) return 0;
      const form = formValue as ReturnFormValue;
      const daysUsed = this.calculateDays(rental.startDate, this.returnDate);
      const endKm = Number(form.endKm ?? 0);
      const kmDriven = Math.max(0, endKm - rental.startKm);

      const planTotalCost = this.money(
        this.calculatePlanPrice(
          rental.billingPlan,
          rental.billingPlanType,
          daysUsed,
          kmDriven,
          rental.estimatedKilometers ?? 0,
        ),
      );
      const extrasTotalCost = this.money(this.calculateExtrasCost(rental.rentalExtras, daysUsed));

      const fuelPenality = this.money(
        this.calculateFuelPenalty(form.fuelLevelAtReturn, rental.vehicle, config),
      );
      const delayPenalty = this.money(
        this.calculateDelayPenalty(planTotalCost, rental.expectedReturnDate, this.returnDate),
      );
      const penaltiesTotalCost = this.money(fuelPenality + delayPenalty);

      const discountTotal = this.money(rental.coupon?.discountValue ?? 0);
      const totalCost = this.money(planTotalCost + extrasTotalCost + penaltiesTotalCost);
      const finalPrice = this.money(Math.max(0, totalCost - discountTotal));

      return finalPrice;
    }),
    startWith(0),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public get endKm(): AbstractControl | null {
    return this.formGroup.get('endKm');
  }

  public get fuelLevelAtReturn(): AbstractControl<fuelLevelAtReturnKey> | null {
    return this.formGroup.get('fuelLevelAtReturn') as AbstractControl<fuelLevelAtReturnKey> | null;
  }

  public return(): void {
    if (this.formGroup.invalid) return;

    const returnModel: RentalReturnDto = this.formGroup.value as RentalReturnDto;

    const returnObserver: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Rental returned successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/rentals']),
    };

    this.rental$
      .pipe(
        take(1),
        switchMap((rental) => this.rentalService.return(rental.id, returnModel)),
      )
      .subscribe(returnObserver);
  }

  private calculateDays(start: Date, end: Date): number {
    const date1 = new Date(start).getTime();
    const date2 = new Date(end).getTime();

    const diffInMs = date2 - date1;
    const diffInDays = diffInMs / (1000 * 60 * 60 * 24);

    const days = Math.ceil(diffInDays);

    return days <= 0 ? 1 : days;
  }

  private calculatePlanPrice(
    billingPlan: BillingPlanDto,
    billingPlanType: string,
    days: number,
    kmDriven: number,
    estimatedKilometers: number,
  ): number {
    switch (billingPlanType) {
      case 'Daily': {
        const daily = billingPlan.dailyBilling;
        if (!daily) return 0;
        return daily.dailyRate * days + daily.pricePerKm * kmDriven;
      }

      case 'Controlled':
        return this.calculateControlledPlan(
          billingPlan.controlledBilling,
          days,
          kmDriven,
          estimatedKilometers ?? 0,
        );

      case 'Free': {
        const free = billingPlan.freeBilling;
        if (!free) return 0;
        return free.fixedRate * days;
      }

      default:
        return 0;
    }
  }

  private calculateControlledPlan(
    plan: ControlledBillingDto,
    days: number,
    kmDriven: number,
    estimatedKm: number,
  ): number {
    if (!plan) return 0;

    const baseCost = plan.dailyRate * days;
    const extraKm = Math.max(0, kmDriven - estimatedKm);

    return baseCost + extraKm * plan.pricePerKmExtrapolated;
  }

  private calculateExtrasCost(extras: Extra[], days: number): number {
    if (!extras || extras.length === 0) return 0;

    return extras.reduce((accumulator, extra) => {
      return accumulator + (extra.isDaily ? extra.price * days : extra.price);
    }, 0);
  }

  private calculateFuelPenalty(
    fuelLevelAtReturn: fuelLevelAtReturnKey | null,
    vehicle: RentalVehicleDto,
    config: Configuration,
  ): number {
    if (!fuelLevelAtReturn) return 0;

    const levelPercent = FUEL_LEVEL_PERCENTAGE[fuelLevelAtReturn] ?? 100;
    if (levelPercent >= 100) return 0;

    const missingLiters = vehicle.fuelTankCapacity * ((100 - levelPercent) / 100);

    const fuelPrices: Record<FuelType, number> = {
      Gasoline: config.gasolinePrice,
      Gas: config.gasPrice,
      Diesel: config.dieselPrice,
      Alcohol: config.alcoholPrice,
    };

    const fuelType = vehicle.fuelType as FuelType;
    const pricePerLiter = fuelPrices[fuelType] ?? 0;

    return missingLiters * pricePerLiter;
  }

  private calculateDelayPenalty(
    planTotal: number,
    expectedReturn: Date,
    actualReturn: Date,
  ): number {
    const expected = new Date(expectedReturn).setHours(0, 0, 0, 0);
    const actual = new Date(actualReturn).setHours(0, 0, 0, 0);

    return actual > expected ? planTotal * 0.1 : 0;
  }

  private money(value: number): number {
    return Math.round((value + Number.EPSILON) * 100) / 100;
  }
}
