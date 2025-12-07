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
  filter,
  map,
  Observable,
  Observer,
  of,
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
import { needsIndividual } from '../../../validators/driver.validators';

@Component({
  selector: 'app-create-driver.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
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

  protected readonly individualCLientsSubject = new BehaviorSubject<DriverIndividualClientDto[]>(
    [],
  );

  protected readonly individualClients$: Observable<DriverIndividualClientDto[]> =
    this.individualCLientsSubject.asObservable();

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

        this.resetDriverFields();

        if (!isBusinessClient && selectedClient) {
          this.toggleManualFields(false);
          this.fillDriverFields(selectedClient);
          this.toggleLicenseFields(true);
          this.formGroup.updateValueAndValidity();
          return of(false);
        }

        if (isBusinessClient && selectedId != null) {
          return this.clientService.getIndividuals(String(selectedId)).pipe(
            tap((individualClients) => {
              this.individualCLientsSubject.next(individualClients);
              const hasExistingIndividuals = individualClients.length > 0;

              if (hasExistingIndividuals) {
                this.toggleIndividualClientId(true);
                this.toggleManualFields(false);
                this.toggleLicenseFields(false);
              } else {
                this.toggleIndividualClientId(false);
                this.toggleManualFields(true);
                this.toggleLicenseFields(true);
              }

              this.formGroup.updateValueAndValidity();
            }),
            map(() => true),
          );
        }

        this.formGroup.updateValueAndValidity();
        return of(false);
      }),
    );
  });

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
      licenseNumber: [
        '',
        [Validators.required.bind(this), Validators.minLength(11), Validators.maxLength(11)],
      ],
      licenseValidity: ['', [Validators.required.bind(this)]],
      clientTypeIsBusiness: [false],
      individualClientId: [null],
    },
    { validators: [needsIndividual] },
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

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: CreateDriverDto = this.formGroup.getRawValue() as CreateDriverDto;

    console.log(registerModel);

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
      licenseValidity: individualClient.licenseValidity,
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

  private toggleIndividualClientId(enable: boolean): void {
    const control = this.individualClientId;

    const hasExistingIndividuals = this.individualCLientsSubject.value.length > 0;

    if (enable) {
      if (hasExistingIndividuals) {
        control?.enable();
      } else {
        control?.disable();
      }
    } else {
      control?.disable();
      control?.setValue(null);
    }

    control?.updateValueAndValidity();
    this.formGroup.updateValueAndValidity();
  }
}
