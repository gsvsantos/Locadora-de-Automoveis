import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, tap, shareReplay, Observer, take, switchMap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { PartnerDetailsDto, PartnerDto } from '../../../models/partner.models';
import { NotificationService } from '../../../services/notification.service';
import { PartnerService } from '../../../services/partner.service';

@Component({
  selector: 'app-update-partner.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-partner.component.html',
  styleUrl: './update-partner.component.scss',
})
export class UpdatePartnerComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly partnerService = inject(PartnerService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly partner$ = this.route.data.pipe(
    filter((data) => data['partner'] as boolean),
    map((data) => data['partner'] as PartnerDetailsDto),
    tap((group: PartnerDetailsDto) => this.formGroup.patchValue(group)),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected formGroup: FormGroup = this.formBuilder.group({
    fullName: [
      '',
      [
        Validators.required.bind(this),
        Validators.minLength(3),
        Validators.pattern(/^[a-zA-ZÄäÖöÜüÀàÈèÌìÒòÙùÁáÉéÍíÓóÚúÝýÂâÊêÎîÔôÛûÃãÑñÇç'\-\s]+$/),
      ],
    ],
  });

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
  }

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: PartnerDto = this.formGroup.value as PartnerDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(`Partner "${updateModel.fullName}" updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/partners']),
    };

    this.partner$
      .pipe(
        take(1),
        switchMap((partner) => this.partnerService.update(partner.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
