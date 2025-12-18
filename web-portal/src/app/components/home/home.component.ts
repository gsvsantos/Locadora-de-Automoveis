import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { Observable, map } from 'rxjs';
import { PagedResult } from '../../models/paged-result.model';
import { Vehicle } from '../../models/vehicle.model';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe, ReactiveFormsModule, RouterLink, TranslocoModule, GsButtons],
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

  protected readonly urlS3: string = 'https://pub-8c88efdf916b4058be00dc36f97c82bf.r2.dev/';
  protected readonly searchControl = new FormControl('');

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
