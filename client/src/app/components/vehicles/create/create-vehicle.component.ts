import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observer, tap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { VehicleDto } from '../../../models/vehicles.models';
import { NotificationService } from '../../../services/notification.service';
import { VehicleService } from './../../../services/vehicle.service';
import { Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';
import { Group } from '../../../models/group.models';

@Component({
  selector: 'app-create-vehicle.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-vehicle.component.html',
  styleUrl: './create-vehicle.component.scss',
})
export class CreateVehicleComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly vehicleService = inject(VehicleService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly groups$ = this.route.data.pipe(
    filter((data) => data['groups'] as boolean),
    map((data) => data['groups'] as Group[]),
    tap((groups) => console.log(groups)),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    groupId: ['', [Validators.required.bind(this)]],
    licensePlate: [
      '',
      [Validators.required.bind(this), Validators.pattern(/^[A-Z]{3}-\d[A-Z\d]\d{2}$/)],
    ],

    brand: ['', [Validators.required.bind(this)]],
    model: ['', [Validators.required.bind(this)]],
    color: ['', [Validators.required.bind(this)]],
    year: [
      '',
      [
        Validators.required.bind(this),
        Validators.min(1950).bind(this),
        Validators.max(new Date().getFullYear() + 1).bind(this),
      ],
    ],

    fuelType: ['', [Validators.required.bind(this)]],
    fuelTankCapacity: ['', [Validators.required.bind(this), Validators.min(1).bind(this)]],

    photoPath: ['', []],
  });

  public get licensePlate(): AbstractControl | null {
    return this.formGroup.get('licensePlate');
  }

  public get groupId(): AbstractControl | null {
    return this.formGroup.get('groupId');
  }

  public get brand(): AbstractControl | null {
    return this.formGroup.get('brand');
  }

  public get model(): AbstractControl | null {
    return this.formGroup.get('model');
  }

  public get color(): AbstractControl | null {
    return this.formGroup.get('color');
  }

  public get year(): AbstractControl | null {
    return this.formGroup.get('year');
  }

  public get fuelType(): AbstractControl | null {
    return this.formGroup.get('fuelType');
  }

  public get fuelTankCapacity(): AbstractControl | null {
    return this.formGroup.get('fuelTankCapacity');
  }

  public get photoPath(): AbstractControl | null {
    return this.formGroup.get('photoPath');
  }

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: VehicleDto = this.formGroup.value as VehicleDto;
    console.log(registerModel);
    const registerObserver: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(
          `Vehicles ${registerModel.licensePlate} registered successfully!`,
        ),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/vehicles']),
    };

    this.vehicleService.register(registerModel).subscribe(registerObserver);
  }
}
