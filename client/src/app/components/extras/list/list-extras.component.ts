import { AsyncPipe, CurrencyPipe, LowerCasePipe, TitleCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { Extra } from '../../../models/extra.models';

@Component({
  selector: 'app-list-extras.component',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    LowerCasePipe,
    TitleCasePipe,
    TranslocoModule,
    RouterLink,
    GsButtons,
  ],
  templateUrl: './list-extras.component.html',
  styleUrl: './list-extras.component.scss',
})
export class ListExtrasComponent {
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

  protected readonly extras$ = this.route.data.pipe(
    filter((data) => data['extras'] as boolean),
    map((data) => data['extras'] as Extra[]),
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
