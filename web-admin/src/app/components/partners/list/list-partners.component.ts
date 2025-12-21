import { AsyncPipe, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { ListPartnerDto } from '../../../models/partner.models';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-list-partners.component',
  imports: [AsyncPipe, TitleCasePipe, TranslocoModule, RouterLink, GsButtons],
  templateUrl: './list-partners.component.html',
  styleUrl: './list-partners.component.scss',
})
export class ListPartnersComponent {
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

  protected readonly partners$ = this.route.data.pipe(
    filter((data) => data['partners'] as boolean),
    map((data) => data['partners'] as ListPartnerDto[]),
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
