import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { BillingPlan } from '../../../models/billing-plan.models';
import { AsyncPipe, CurrencyPipe, TitleCasePipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-list-billing-plans.component',
  imports: [AsyncPipe, CurrencyPipe, TitleCasePipe, TranslocoModule, RouterLink, GsButtons],
  templateUrl: './list-billing-plans.component.html',
  styleUrl: './list-billing-plans.component.scss',
})
export class ListBillingPlansComponent {
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

  protected readonly billingPlans$ = this.route.data.pipe(
    filter((data) => data['billingPlans'] as boolean),
    map((data) => data['billingPlans'] as BillingPlan[]),
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
