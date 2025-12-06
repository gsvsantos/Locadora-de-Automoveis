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
import { NotificationService } from '../../../services/notification.service';
import { GroupService } from '../../../services/group.service';
import { GroupDto } from '../../../models/group.models';
import { IdApiResponse } from '../../../models/api.models';
import { Observer } from 'rxjs';

@Component({
  selector: 'app-create-group.component',
  imports: [RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './create-group.component.html',
  styleUrl: './create-group.component.scss',
})
export class CreateGroupComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly groupService = inject(GroupService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected formGroup: FormGroup = this.formBuilder.group({
    name: [
      '',
      [
        Validators.required.bind(this),
        Validators.minLength(3),
        Validators.pattern(/^[a-zA-ZÄäÖöÜüÀàÈèÌìÒòÙùÁáÉéÍíÓóÚúÝýÂâÊêÎîÔôÛûÃãÑñÇç'\-\s]+$/),
      ],
    ],
  });

  public get name(): AbstractControl | null {
    return this.formGroup.get('name');
  }

  public register(): void {
    if (this.formGroup.invalid) return;

    const registerModel: GroupDto = this.formGroup.value as GroupDto;

    const registerObserver: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(`Group "${registerModel.name}" registered successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/groups']),
    };

    this.groupService.register(registerModel).subscribe(registerObserver);
  }
}
