import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay } from 'rxjs';
import { PartnerDetailsDto } from '../../../models/partner.models';

@Component({
  selector: 'app-get-coupons-partner.component',
  imports: [AsyncPipe, CurrencyPipe, DatePipe, TranslocoModule, RouterLink, GsButtons],
  templateUrl: './get-coupons-partner.component.html',
  styleUrl: './get-coupons-partner.component.scss',
})
export class GetCouponsPartnerComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly partner$ = this.route.data.pipe(
    filter((data) => data['partner'] as boolean),
    map((data) => data['partner'] as PartnerDetailsDto),
    shareReplay({ bufferSize: 1, refCount: true }),
  );
}
