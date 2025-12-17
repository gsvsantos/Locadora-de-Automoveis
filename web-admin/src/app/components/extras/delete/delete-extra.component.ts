import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, shareReplay, Observer, take, switchMap } from 'rxjs';
import { Extra } from '../../../models/extra.models';
import { ExtraService } from '../../../services/extra.service';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe, CurrencyPipe, LowerCasePipe } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-extra.component',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    LowerCasePipe,
    RouterLink,
    ReactiveFormsModule,
    TranslocoModule,
    GsButtons,
  ],
  templateUrl: './delete-extra.component.html',
  styleUrl: './delete-extra.component.scss',
})
export class DeleteExtraComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly extraService = inject(ExtraService);
  protected readonly notificationService = inject(NotificationService);
  protected readonly buttonType = gsButtonTypeEnum;
  protected readonly targetType = gsTabTargetEnum;
  protected readonly variantType = gsVariant;

  protected readonly extra$ = this.route.data.pipe(
    filter((data) => data['extra'] as boolean),
    map((data) => data['extra'] as Extra),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/extras']),
    };

    this.extra$
      .pipe(
        take(1),
        switchMap((extra) => this.extraService.delete(extra.id)),
      )
      .subscribe(deleteObserver);
  }
}
