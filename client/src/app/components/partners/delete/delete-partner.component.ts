import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { PartnerDetailsDto } from '../../../models/partner.models';
import { NotificationService } from '../../../services/notification.service';
import { PartnerService } from '../../../services/partner.service';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-partner.component',
  imports: [AsyncPipe, RouterLink, TranslocoModule, GsButtons],
  templateUrl: './delete-partner.component.html',
  styleUrl: './delete-partner.component.scss',
})
export class DeletePartnerComponent {
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
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/partners']),
    };

    this.partner$
      .pipe(
        take(1),
        switchMap((partner) => this.partnerService.delete(partner.id)),
      )
      .subscribe(deleteObserver);
  }
}
