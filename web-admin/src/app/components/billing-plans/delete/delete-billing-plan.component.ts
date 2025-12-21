import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { BillingPlan } from '../../../models/billing-plan.models';
import { BillingPlanService } from '../../../services/billing-plan.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-delete-billing-plan.component',
  imports: [AsyncPipe, CurrencyPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './delete-billing-plan.component.html',
  styleUrl: './delete-billing-plan.component.scss',
})
export class DeleteBillingPlanComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly billingPlanService = inject(BillingPlanService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly billingPlan$ = this.route.data.pipe(
    filter((data) => data['billingPlan'] as boolean),
    map((data) => data['billingPlan'] as BillingPlan),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/billing-plans']),
    };

    this.billingPlan$
      .pipe(
        take(1),
        switchMap((billingPlan) => this.billingPlanService.delete(billingPlan.id)),
      )
      .subscribe(deleteObserver);
  }
}
