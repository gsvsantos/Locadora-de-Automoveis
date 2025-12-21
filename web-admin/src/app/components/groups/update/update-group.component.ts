import { Component, inject } from '@angular/core';
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
import { GroupService } from '../../../services/group.service';
import { NotificationService } from '../../../services/notification.service';
import { Group, GroupDto } from '../../../models/group.models';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-update-group.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './update-group.component.html',
  styleUrl: './update-group.component.scss',
})
export class UpdateGroupComponent {
  protected readonly formBuilder = inject(FormBuilder);
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly groupService = inject(GroupService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly group$ = this.route.data.pipe(
    filter((data) => data['group'] as boolean),
    map((data) => data['group'] as Group),
    tap((group: Group) => this.formGroup.patchValue(group)),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

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

  public update(): void {
    if (this.formGroup.invalid) return;

    const updateModel: GroupDto = this.formGroup.value as GroupDto;

    const updateObserve: Observer<IdApiResponse> = {
      next: () =>
        this.notificationService.success(`Group "${updateModel.name}" updated successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/groups']),
    };

    this.group$
      .pipe(
        take(1),
        switchMap((group) => this.groupService.update(group.id, updateModel)),
      )
      .subscribe(updateObserve);
  }
}
