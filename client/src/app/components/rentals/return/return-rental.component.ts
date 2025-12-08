import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  AbstractControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { RentalDetailsDto, ReturnRentalDto } from '../../../models/rental.models';
import { NotificationService } from '../../../services/notification.service';
import { RentalService } from '../../../services/rental.service';
import { IdApiResponse } from '../../../models/api.models';

@Component({
  selector: 'app-return-rental.component',
  imports: [AsyncPipe, DatePipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './return-rental.component.html',
  styleUrl: './return-rental.component.scss',
})
export class ReturnRentalComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly rentalService = inject(RentalService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly rental$ = this.route.data.pipe(
    filter((data) => data['rental'] as boolean),
    map((data) => data['rental'] as RentalDetailsDto),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    endKm: ['', [Validators.required.bind(this)]],
    fuelLevel: ['', [Validators.required.bind(this)]],
  });

  public get endKm(): AbstractControl | null {
    return this.formGroup.get('endKm');
  }

  public get fuelLevel(): AbstractControl | null {
    return this.formGroup.get('fuelLevel');
  }

  public return(): void {
    if (this.formGroup.invalid) return;

    const returnModel: ReturnRentalDto = this.formGroup.value as ReturnRentalDto;

    const returnObserver: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Rental returned successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/rentals']),
    };

    this.rental$
      .pipe(
        take(1),
        switchMap((rental) => this.rentalService.return(rental.id, returnModel)),
      )
      .subscribe(returnObserver);
  }
}
