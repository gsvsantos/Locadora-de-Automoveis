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
import { ExtraDto } from '../../../models/extra.models';
import { NotificationService } from '../../../services/notification.service';
import { ExtraService } from './../../../services/extra.service';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-create-extra.component',
  imports: [RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-extra.component.html',
  styleUrl: './create-extra.component.scss',
})
export class CreateExtraComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly extraService = inject(ExtraService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected formGroup: FormGroup = this.formBuilder.group({
    name: ['', [Validators.required.bind(this), Validators.minLength(3)]],
    price: ['', [Validators.required.bind(this), Validators.min(1)]],
    isDaily: [false, []],
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

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: ExtraDto = this.formGroup.value as ExtraDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Extra registered successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/extras']),
    };

    this.extraService.register(registerModel).subscribe(registerObserver);
  }
}
