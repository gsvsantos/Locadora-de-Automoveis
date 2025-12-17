import { Component, inject } from '@angular/core';
import { PartnerService } from '../../../services/partner.service';
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
import { Observer } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { PartnerDto } from '../../../models/partner.models';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-create-partner.component',
  imports: [RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-partner.component.html',
  styleUrl: './create-partner.component.scss',
})
export class CreatePartnerComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly partnerService = inject(PartnerService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

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

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: PartnerDto = this.formGroup.value as PartnerDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(
          `Partner "${registerModel.fullName}" registered successfully!`,
        ),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/partners']),
    };

    this.partnerService.register(registerModel).subscribe(registerObserver);
  }
}
