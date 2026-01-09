/* eslint-disable no-useless-escape */
import { AsyncPipe } from '@angular/common';
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
import {
  BehaviorSubject,
  defer,
  delay,
  filter,
  map,
  merge,
  Observable,
  Observer,
  of,
  shareReplay,
  startWith,
  switchMap,
  tap,
  withLatestFrom,
} from 'rxjs';
import { IdApiResponse } from '../../../models/api.models';
import { CreateDriverDto, DriverIndividualClientDto } from '../../../models/driver.models';
import { NotificationService } from '../../../services/notification.service';
import { DriverService } from './../../../services/driver.service';
import { Component, inject } from '@angular/core';
import { Client } from '../../../models/client.models';
import { ClientService } from '../../../services/client.service';
import { needsIndividualValidator } from '../../../validators/driver.validators';
import { dateToInputDateString } from '../../../utils/date.utils';
import { NgxMaskDirective, NgxMaskPipe } from 'ngx-mask';

@Component({
  selector: 'app-create-driver.component',
  imports: [
    AsyncPipe,
    RouterLink,
    NgxMaskDirective,
    NgxMaskPipe,
    ReactiveFormsModule,
    TranslocoModule,
    GsButtons,
  ],
  templateUrl: './create-driver.component.html',
  styleUrl: './create-driver.component.scss',
})
export class CreateDriverComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly clientService = inject(ClientService);
  protected readonly driverService = inject(DriverService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly clients$ = this.route.data.pipe(
    filter((data) => data['clients'] as boolean),
    map((data) => data['clients'] as Client[]),
  );

  protected readonly individualClientsSubject = new BehaviorSubject<DriverIndividualClientDto[]>(
    [],
  );

  protected readonly individualClients$: Observable<DriverIndividualClientDto[]> =
    this.individualClientsSubject.asObservable();

  protected readonly selectedIndividualClient$: Observable<DriverIndividualClientDto | null> =
    defer(() => {
      const individualClientIdControl = this.individualClientId;

      if (!individualClientIdControl) {
        return of(null);
      }

      return individualClientIdControl.valueChanges.pipe(
        startWith(individualClientIdControl.value),
        withLatestFrom(this.individualClients$),
        map(([selectedIndividualId, individuals]: [string | null, DriverIndividualClientDto[]]) => {
          if (!selectedIndividualId) {
            return null;
          }
          return individuals.find((i) => i.id === selectedIndividualId) || null;
        }),
        tap((individualClient: DriverIndividualClientDto | null) => {
          if (individualClient) {
            this.fillDriverFieldsFromIndividual(individualClient);
            this.toggleLicenseFields(true);
          } else {
            this.resetDriverFields();
            this.toggleLicenseFields(false);
          }
        }),
      );
    });

  protected readonly toggleNewDriverMode$: Observable<boolean> = defer(() => {
    const newDriverControl = this.registerNewDriver;

    if (!newDriverControl) {
      return of(false);
    }

    return newDriverControl.valueChanges.pipe(
      startWith(newDriverControl.value),
      delay(0),
      tap((isRegisteringNew: boolean) => {
        if (isRegisteringNew) {
          this.individualClientId?.setValue(null, { emitEvent: false });
          this.individualClientId?.disable({ emitEvent: false });

          this.resetDriverFields();
          this.toggleManualFields(true);
          this.toggleLicenseFields(true);
        } else {
          this.toggleManualFields(false);

          if (this.individualClientsSubject.value.length > 0) {
            this.individualClientId?.enable({ emitEvent: false });
          } else {
            this.individualClientId?.disable({ emitEvent: false });
          }

          if (!this.individualClientId?.value) {
            this.resetDriverFields();
          }
        }
      }),
    );
  });

  protected readonly clientTypeIsBusiness$: Observable<boolean> = defer(() => {
    const clientIdControl = this.clientId;

    if (!clientIdControl) {
      return of(false);
    }

    return clientIdControl.valueChanges.pipe(
      startWith(clientIdControl.value),
      withLatestFrom(this.clients$),

      map(([selectedId, clients]: [string | number | null, Client[]]) => {
        const selectedClient = clients.find((client) => String(client.id) === String(selectedId));

        const isBusinessClient = selectedClient?.type === 'Business';
        return { selectedId, isBusinessClient, selectedClient };
      }),

      switchMap(({ selectedId, isBusinessClient, selectedClient }) => {
        this.formGroup.get('clientTypeIsBusiness')?.setValue(isBusinessClient);

        this.registerNewDriver?.setValue(false, { emitEvent: false });
        this.registerNewDriver?.enable({ emitEvent: false });
        this.individualClientId?.setValue(null, { emitEvent: false });
        this.individualClientsSubject.next([]);
        this.resetDriverFields();
        this.toggleManualFields(false);

        if (!isBusinessClient && selectedClient) {
          this.fillDriverFields(selectedClient);
          this.toggleLicenseFields(true);
          this.formGroup.updateValueAndValidity();
          return of(false);
        }

        if (isBusinessClient && selectedId != null) {
          const individualClientsLoaded$: Observable<boolean> = this.clientService
            .getIndividuals(String(selectedId))
            .pipe(
              tap((individualClients) => {
                this.individualClientsSubject.next(individualClients);
                const hasExistingIndividuals: boolean = individualClients.length > 0;

                if (hasExistingIndividuals) {
                  this.registerNewDriver?.setValue(false, { emitEvent: true });
                } else {
                  this.registerNewDriver?.setValue(true, { emitEvent: true });
                  this.registerNewDriver?.disable({ emitEvent: false });
                }

                this.formGroup.updateValueAndValidity();
              }),
              map(() => true),
            );

          const selectedClientLogged$: Observable<boolean> = this.selectedIndividualClient$.pipe(
            tap((selected) => {
              if (selected) {
                this.fillDriverFieldsFromIndividual(selected);
                this.formGroup.updateValueAndValidity();
              }
            }),
            map(() => true),
          );

          return merge(
            individualClientsLoaded$,
            selectedClientLogged$,
            this.toggleNewDriverMode$,
          ).pipe(
            startWith(true),
            map(() => true),
            shareReplay({ bufferSize: 1, refCount: true }),
          );
        }

        this.formGroup.updateValueAndValidity();
        return of(false);
      }),
    );
  }).pipe(shareReplay({ bufferSize: 1, refCount: true }));

  protected formGroup: FormGroup = this.formBuilder.group(
    {
      clientId: [null, [Validators.required.bind(this)]],
      fullName: [
        '',
        [
          Validators.required.bind(this),
          Validators.minLength(3),
          Validators.pattern(/^[a-zA-ZÄäÖöÜüÀàÈèÌìÒòÙùÁáÉéÍíÓóÚúÝýÂâÊêÎîÔôÛûÃãÑñÇç'\-\s]+$/),
        ],
      ],
      document: [
        '',
        [
          Validators.required.bind(this),
          Validators.pattern(/^([0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2})$/),
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
      licenseNumber: [
        '',
        [Validators.required.bind(this), Validators.minLength(11), Validators.maxLength(11)],
      ],
      licenseValidity: ['', [Validators.required.bind(this)]],
      clientTypeIsBusiness: [false],
      registerNewDriver: [false],
      individualClientId: [null],
    },
    { validators: [needsIndividualValidator] },
  );

  public get clientId(): AbstractControl | null {
    return this.formGroup.get('clientId');
  }

  public get fullName(): AbstractControl | null {
    return this.formGroup.get('fullName');
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

  public get licenseNumber(): AbstractControl | null {
    return this.formGroup.get('licenseNumber');
  }

  public get licenseValidity(): AbstractControl | null {
    return this.formGroup.get('licenseValidity');
  }

  public get individualClientId(): AbstractControl | null {
    return this.formGroup.get('individualClientId');
  }

  public get clientTypeIsBusiness(): AbstractControl | null {
    return this.formGroup.get('clientTypeIsBusiness');
  }

  public get registerNewDriver(): AbstractControl | null {
    return this.formGroup.get('registerNewDriver');
  }

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: CreateDriverDto = this.formGroup.getRawValue() as CreateDriverDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () => this.notificationService.success(`Driver registered successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/drivers']),
    };

    this.driverService.register(registerModel).subscribe(registerObserver);
  }

  private resetDriverFields(): void {
    const fieldsToReset = [
      'fullName',
      'document',
      'email',
      'phoneNumber',
      'licenseNumber',
      'licenseValidity',
    ];
    fieldsToReset.forEach((field) => {
      this.formGroup.get(field)?.setValue('');
    });
  }

  private fillDriverFields(client: Client): void {
    this.formGroup.patchValue({
      fullName: client.fullName,
      document: client.document,
      email: client.email,
      phoneNumber: client.phoneNumber,
    });
  }

  private fillDriverFieldsFromIndividual(individualClient: DriverIndividualClientDto): void {
    this.formGroup.patchValue({
      fullName: individualClient.fullName,
      document: individualClient.document,
      email: individualClient.email,
      phoneNumber: individualClient.phoneNumber,
      licenseNumber: individualClient.licenseNumber,
      licenseValidity: dateToInputDateString(individualClient.licenseValidity),
    });
  }

  private toggleLicenseFields(enable: boolean): void {
    const fields = ['licenseNumber', 'licenseValidity'];
    fields.forEach((field) => {
      const control = this.formGroup.get(field);
      if (enable) {
        control?.enable();
        control?.addValidators(Validators.required.bind(this));
      } else {
        control?.disable();
        control?.removeValidators(Validators.required.bind(this));
      }
      control?.updateValueAndValidity();
    });
  }

  private toggleManualFields(enable: boolean): void {
    const manualFields: string[] = ['fullName', 'document', 'email', 'phoneNumber'];

    manualFields.forEach((fieldName: string) => {
      const control = this.formGroup.get(fieldName);

      if (!control) return;

      if (enable) {
        control.enable();
        control.addValidators(Validators.required.bind(this));
      } else {
        control.disable();
        control.removeValidators(Validators.required.bind(this));
      }

      control.updateValueAndValidity();
    });
  }
}
