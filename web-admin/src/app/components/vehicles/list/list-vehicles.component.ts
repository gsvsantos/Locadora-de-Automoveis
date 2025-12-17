import { AsyncPipe, TitleCasePipe, UpperCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { map, filter } from 'rxjs';
import { Vehicle } from '../../../models/vehicles.models';

@Component({
  selector: 'app-list-vehicles.component',
  imports: [AsyncPipe, UpperCasePipe, TitleCasePipe, TranslocoModule, RouterLink, GsButtons],
  templateUrl: './list-vehicles.component.html',
  styleUrl: './list-vehicles.component.scss',
})
export class ListVehiclesComponent {
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

  protected readonly vehicles$ = this.route.data.pipe(
    filter((data) => data['vehicles'] as boolean),
    map((data) => data['vehicles'] as Vehicle[]),
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
