import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { filter, map, shareReplay } from 'rxjs';
import { ClientProfile } from '../../../models/account.models';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from "gs-buttons";

@Component({
  selector: 'app-details-account.component',
  imports: [AsyncPipe, TranslocoModule, GsButtons, RouterLink],
  templateUrl: './details-account.component.html',
  styleUrl: './details-account.component.scss',
})
export class DetailsAccountComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly account$ = this.route.data.pipe(
    filter((data) => data['account'] as boolean),
    map((data) => data['account'] as ClientProfile),
    shareReplay({ bufferSize: 1, refCount: true }),
  );
}
