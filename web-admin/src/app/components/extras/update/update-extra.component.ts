import { Extra } from './../../../models/extra.models';
import { Component, inject } from '@angular/core';
import { ExtraDto } from '../../../models/extra.models';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, tap, shareReplay, Observer, take, switchMap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { ExtraService } from '../../../services/extra.service';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-update-extra.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-extra.component.html',
  styleUrl: './update-extra.component.scss',
})
export class UpdateExtraComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly extraService = inject(ExtraService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly extra$ = this.route.data.pipe(
    filter((data) => data['extra'] as boolean),
    map((data) => data['extra'] as Extra),
    tap((extra: Extra) => this.formGroup.patchValue(extra)),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    name: ['', [Validators.required.bind(this), Validators.minLength(3)]],
    price: ['', [Validators.required.bind(this), Validators.min(1)]],
    isDaily: ['', []],
    type: ['', [Validators.required.bind(this)]],
  });

  public get name(): AbstractControl | null {
    return this.formGroup.get('name');
  }

  public get price(): AbstractControl | null {
    return this.formGroup.get('price');
  }

  public get isDaily(): AbstractControl | null {
    return this.formGroup.get('isDaily');
  }

  public get type(): AbstractControl | null {
    return this.formGroup.get('type');
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: ExtraDto = this.formGroup.value as ExtraDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Extra updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/extras']),
    };

    this.extra$
      .pipe(
        take(1),
        switchMap((extra) => this.extraService.update(extra.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
