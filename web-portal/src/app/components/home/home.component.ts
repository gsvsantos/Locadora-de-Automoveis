import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule, TranslocoService } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import {
  Observable,
  combineLatest,
  debounceTime,
  distinctUntilChanged,
  filter,
  map,
  shareReplay,
  startWith,
  switchMap,
} from 'rxjs';
import { PagedResult } from '../../models/paged-result.models';
import { Vehicle } from '../../models/vehicle.models';
import { VehicleCardComponent } from '../vehicle-card/vehicle-card.component';
import { VehicleService } from '../../services/vehicle.service';
import { Group } from '../../models/group.models';
import { GroupService } from '../../services/group.service';
import { MyRentalStatusDto } from '../../models/rental.models';
import { RentalService } from '../../services/rental.service';

@Component({
  selector: 'app-home',
  imports: [AsyncPipe, ReactiveFormsModule, TranslocoModule, GsButtons, VehicleCardComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home {
  protected readonly translocoService = inject(TranslocoService);
  protected readonly vehicleService = inject(VehicleService);
  protected readonly groupService = inject(GroupService);
  protected readonly rentalService = inject(RentalService);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly Math = Math;
  protected readonly urlS3: string = 'https://pub-8c88efdf916b4058be00dc36f97c82bf.r2.dev/';

  protected readonly searchControl = new FormControl('');
  protected readonly groupIdControl = new FormControl<string | null>(null);
  protected readonly fuelTypeControl = new FormControl<string | null>(null);
  protected currentPage = 1;

  protected readonly queryText$ = this.searchControl.valueChanges.pipe(
    startWith(this.searchControl.value ?? ''),
    debounceTime(300),
    distinctUntilChanged(),
    map((value) => (value || '').trim()),
  );

  protected readonly groupId$ = this.groupIdControl.valueChanges.pipe(
    startWith(this.groupIdControl.value ?? undefined),
    distinctUntilChanged(),
  );

  protected readonly fuelType$ = this.fuelTypeControl.valueChanges.pipe(
    startWith(this.fuelTypeControl.value ?? undefined),
    distinctUntilChanged(),
  );

  protected readonly groups$ = this.route.data.pipe(
    filter((data) => data['groups'] as boolean),
    map((data) => data['groups'] as Group[]),
  );

  protected readonly rentalStatus$: Observable<MyRentalStatusDto> = this.rentalService
    .getMyRentalStatus()
    .pipe(shareReplay({ bufferSize: 1, refCount: true }));

  protected readonly state$: Observable<PagedResult<Vehicle>> = combineLatest([
    this.queryText$,
    this.groupId$,
    this.fuelType$,
  ]).pipe(
    switchMap(([term, groupId, fuelType]) =>
      this.vehicleService.getAvailableVehicles(
        this.currentPage,
        10,
        term || undefined,
        groupId || undefined,
        fuelType || undefined,
      ),
    ),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly viewModel$ = combineLatest([this.state$, this.rentalStatus$]).pipe(
    map(([state, rentalStatus]) => ({ state, rentalStatus })),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected onPageChange(newPage: number): void {
    this.currentPage = newPage;
    this.searchControl.setValue(this.searchControl.value ?? '', { emitEvent: true });
  }
}
