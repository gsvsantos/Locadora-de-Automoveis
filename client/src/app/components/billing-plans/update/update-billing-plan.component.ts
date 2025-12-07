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
import { filter, map, tap, Observer, take, switchMap, shareReplay } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { Group } from '../../../models/group.models';
import { BillingPlanService } from '../../../services/billing-plan.service';
import { NotificationService } from '../../../services/notification.service';
import { BillingPlan, BillingPlanDto } from '../../../models/billing-plan.models';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-update-billing-plan.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-billing-plan.component.html',
  styleUrl: './update-billing-plan.component.scss',
})
export class UpdateBillingPlanComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly billingPlanService = inject(BillingPlanService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly groups$ = this.route.data.pipe(
    filter((data) => data['groups'] as boolean),
    map((data) => data['groups'] as Group[]),
  );

  protected readonly billingPlan$ = this.route.data.pipe(
    filter((data) => data['billingPlan'] as boolean),
    map((data) => data['billingPlan'] as BillingPlan),
    tap((billingPlan: BillingPlan) =>
      this.formGroup.patchValue({ ...billingPlan, groupId: billingPlan.groupId }),
    ),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    groupId: ['', Validators.required.bind(this)],
    dailyBilling: this.formBuilder.group({
      dailyRate: ['', Validators.min(0.01)],
      pricePerKm: ['', Validators.min(0.01)],
    }),
    controlledBilling: this.formBuilder.group({
      dailyRate: ['', Validators.min(0.01)],
      pricePerKmExtrapolated: ['', Validators.min(0.01)],
    }),
    freeBilling: this.formBuilder.group({
      fixedRate: ['', Validators.min(0.01)],
    }),
  });

  public get groupId(): AbstractControl | null {
    return this.formGroup.get('groupId');
  }

  public get dailyBilling(): AbstractControl | null {
    return this.formGroup.get('dailyBilling');
  }

  public get controlledBilling(): AbstractControl | null {
    return this.formGroup.get('controlledBilling');
  }

  public get freeBilling(): AbstractControl | null {
    return this.formGroup.get('freeBilling');
  }

  public get dailyRateDaily(): AbstractControl | null {
    return this.dailyBilling?.get('dailyRate') ?? null;
  }

  public get pricePerKm(): AbstractControl | null {
    return this.dailyBilling?.get('pricePerKm') ?? null;
  }

  public get dailyRateControlled(): AbstractControl | null {
    return this.controlledBilling?.get('dailyRate') ?? null;
  }

  public get pricePerKmExtrapolated(): AbstractControl | null {
    return this.controlledBilling?.get('pricePerKmExtrapolated') ?? null;
  }

  public get fixedRate(): AbstractControl | null {
    return this.freeBilling?.get('fixedRate') ?? null;
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: BillingPlanDto = this.formGroup.value as BillingPlanDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Billing Plan updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/billing-plans']),
    };

    this.billingPlan$
      .pipe(
        take(1),
        switchMap((billingPlan) => this.billingPlanService.update(billingPlan.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
