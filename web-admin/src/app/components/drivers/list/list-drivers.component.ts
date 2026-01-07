import { Component, inject } from '@angular/core';
import { Driver } from '../../../models/driver.models';
import { AsyncPipe, DatePipe, TitleCasePipe } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { NgxMaskPipe } from 'ngx-mask';

@Component({
  selector: 'app-list-drivers.component',
  imports: [
    AsyncPipe,
    DatePipe,
    NgxMaskPipe,
    TitleCasePipe,
    TranslocoModule,
    RouterLink,
    GsButtons,
  ],
  templateUrl: './list-drivers.component.html',
  styleUrl: './list-drivers.component.scss',
})
export class ListDriversComponent {
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

  protected readonly drivers$ = this.route.data.pipe(
    filter((data) => data['drivers'] as boolean),
    map((data) => data['drivers'] as Driver[]),
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
