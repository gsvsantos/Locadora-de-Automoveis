import { AsyncPipe } from '@angular/common';
/* eslint-disable no-useless-escape */
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
import { Observer, take, switchMap, filter, map, shareReplay, tap } from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { ClientService } from '../../../services/client.service';
import { NotificationService } from '../../../services/notification.service';
import { Client, ClientDto } from '../../../models/client.models';

@Component({
  selector: 'app-update-client.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-client.component.html',
  styleUrl: './update-client.component.scss',
})
export class UpdateClientComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly clientService = inject(ClientService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly client$ = this.route.data.pipe(
    filter((data) => data['client'] as boolean),
    map((data) => data['client'] as Client),
    tap((client: Client) =>
      this.formGroup.patchValue({
        ...client,
        state: client.address.state,
        city: client.address.city,
        neighborhood: client.address.neighborhood,
        street: client.address.street,
        number: client.address.number,
      }),
    ),
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
    type: ['', [Validators.required.bind(this)]],
    document: [
      '',
      [
        Validators.required.bind(this),
        Validators.pattern(
          /^([0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2}|[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2})$/,
        ),
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
    state: ['', [Validators.required.bind(this)]],
    city: ['', [Validators.required.bind(this)]],
    neighborhood: ['', [Validators.required.bind(this)]],
    street: ['', [Validators.required.bind(this)]],
    number: ['', [Validators.required.bind(this)]],
  });

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
  }

  public get type(): AbstractControl | null {
    return this.formGroup.get('type');
  }

  public get document(): AbstractControl | null {
    return this.formGroup.get('document');
  }

  public get email(): AbstractControl | null {
    return this.formGroup.get('email');
  }

  public get phoneNumber(): AbstractControl | null {
    return this.formGroup.get('phoneNumber');
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
    if (this.formGroup.invalid) return;

    const updateModel: ClientDto = this.formGroup.value as ClientDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Client updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/clients']),
    };

    this.client$
      .pipe(
        take(1),
        switchMap((client) => this.clientService.update(client.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
