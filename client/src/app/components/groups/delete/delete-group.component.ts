import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { GroupService } from '../../../services/group.service';
import { NotificationService } from '../../../services/notification.service';
import { Group } from '../../../models/group.models';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-group.component',
  imports: [AsyncPipe, RouterLink, TranslocoModule, GsButtons],
  templateUrl: './delete-group.component.html',
  styleUrl: './delete-group.component.scss',
})
export class DeleteGroupComponent {
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
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/groups']),
    };

    this.group$
      .pipe(
        take(1),
        switchMap((group) => this.groupService.delete(group.id)),
      )
      .subscribe(deleteObserver);
  }
}
