/* eslint-disable no-useless-escape */
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
import { filter, map, tap, shareReplay, Observer, take } from 'rxjs';
import { ClientProfile } from '../../../models/account.models';
import { IdApiResponse } from '../../../models/api.models';
import { AccountService } from '../../../services/account.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-edit-account.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './edit-account.component.html',
  styleUrl: './edit-account.component.scss',
})
export class EditAccountComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly accountService = inject(AccountService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly account$ = this.route.data.pipe(
    filter((data) => data['account'] as boolean),
    map((data) => data['account'] as ClientProfile),
    tap((client) => this.fillForm(client)),
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
    email: ['', [Validators.required.bind(this), Validators.email.bind(this)]],
    phoneNumber: [
      '',
      [
        Validators.required.bind(this),
        Validators.pattern(/^\(?\d{2}\)?\s?(9\d{4}|\d{4})-?\d{4}$/).bind(this),
      ],
    ],
    document: [
      '',
      [
        Validators.pattern(
          /^([0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2}|[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2})$/,
        ),
      ],
    ],
    state: [''],
    city: [''],
    neighborhood: [''],
    street: [''],
    number: [''],
  });

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
  }
  public get email(): AbstractControl | null {
    return this.formGroup.get('email');
  }
  public get phoneNumber(): AbstractControl | null {
    return this.formGroup.get('phoneNumber');
  }
  public get document(): AbstractControl | null {
    return this.formGroup.get('document');
  }
  public get state(): AbstractControl | null {
    return this.formGroup.get('state');
  }
  public get city(): AbstractControl | null {
    return this.formGroup.get('city');
  }
  public get neighborhood(): AbstractControl | null {
    return this.formGroup.get('neighborhood');
  }
  public get street(): AbstractControl | null {
    return this.formGroup.get('street');
  }
  public get number(): AbstractControl | null {
    return this.formGroup.get('number');
  }

  public update(): void {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    const updateModel: ClientProfile = this.formGroup.value as ClientProfile;

    const updateObserver: Observer<null> = {
      next: () => this.notificationService.success(`Profile updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/home']),
    };

    this.accountService.updateProfile(updateModel).pipe(take(1)).subscribe(updateObserver);
  }

  private fillForm(client: ClientProfile): void {
    this.formGroup.patchValue({
      fullName: client.fullName,
      email: client.email,
      phoneNumber: client.phoneNumber,
      document: client.document,
      state: client.address?.state || '',
      city: client.address?.city || '',
      neighborhood: client.address?.neighborhood || '',
      street: client.address?.street || '',
      number: client.address?.number || '',
    });
  }
}
