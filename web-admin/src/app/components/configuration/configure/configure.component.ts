import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { ConfigurationService } from '../../../services/configuration.service';
import { Configuration, ConfigurationDto } from '../../../models/configuration.models';
import { filter, map, tap, shareReplay, Observer, switchMap, take } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-configure.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './configure.component.html',
  styleUrl: './configure.component.scss',
})
export class ConfigureComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly configurationService = inject(ConfigurationService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly configuration$ = this.route.data.pipe(
    filter((data) => data['configuration'] as boolean),
    map((data) => data['configuration'] as Configuration),
    tap((configuration: Configuration) => this.formGroup.patchValue(configuration)),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    gasolinePrice: ['', [Validators.required.bind(this), Validators.min(1)]],
    gasPrice: ['', [Validators.required.bind(this), Validators.min(1)]],
    dieselPrice: ['', [Validators.required.bind(this), Validators.min(1)]],
    alcoholPrice: ['', [Validators.required.bind(this), Validators.min(1)]],
  });

  public get gasolinePrice(): AbstractControl | null {
    return this.formGroup.get('gasolinePrice');
  }

  public get gasPrice(): AbstractControl | null {
    return this.formGroup.get('gasPrice');
  }

  public get dieselPrice(): AbstractControl | null {
    return this.formGroup.get('dieselPrice');
  }

  public get alcoholPrice(): AbstractControl | null {
    return this.formGroup.get('alcoholPrice');
  }

  public submit(): void {
    if (this.formGroup.invalid) return;

    const configureModel: ConfigurationDto = this.formGroup.value as ConfigurationDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Configured successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/home']),
    };

    this.configuration$
      .pipe(
        take(1),
        switchMap(() => this.configurationService.configure(configureModel)),
      )
      .subscribe(updateObserve);
  }
}
