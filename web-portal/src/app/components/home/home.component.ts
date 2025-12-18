import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { Observable, map } from 'rxjs';
import { PagedResult } from '../../models/paged-result.model';
import { Vehicle } from '../../models/vehicle.model';
import { VehicleCardComponent } from '../vehicle-card/vehicle-card.component';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe, ReactiveFormsModule, TranslocoModule, GsButtons, VehicleCardComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home {
  protected readonly translocoService = inject(TranslocoService);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly Math = Math;
  protected readonly urlS3: string = 'https://pub-8c88efdf916b4058be00dc36f97c82bf.r2.dev/';
  protected readonly searchControl = new FormControl(
    this.route.snapshot.queryParams['searchTerm'] || '',
  );

  protected readonly state$: Observable<PagedResult<Vehicle>> = this.route.data.pipe(
    map((data) => data['pagedVehicles'] as PagedResult<Vehicle>),
  );

  protected updateParams(queryParams: unknown): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams!,
      queryParamsHandling: 'merge',
    });
  }

  protected onPageChange(newPage: number): void {
    this.updateParams({ page: newPage });
  }
}
