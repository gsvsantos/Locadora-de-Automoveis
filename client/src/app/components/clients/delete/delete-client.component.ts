import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GsButtons, gsButtonTypeEnum, gsTabTargetEnum, gsVariant } from 'gs-buttons';
import { filter, map, Observer, take, switchMap, shareReplay } from 'rxjs';
import { Client } from '../../../models/client.models';
import { ClientService } from '../../../services/client.service';
import { NotificationService } from '../../../services/notification.service';
import { AsyncPipe } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-delete-client.component',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, TranslocoModule, GsButtons],
  templateUrl: './delete-client.component.html',
  styleUrl: './delete-client.component.scss',
})
export class DeleteClientComponent {
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
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public delete(): void {
    const deleteObserver: Observer<null> = {
      next: () => this.notificationService.success(`Record deleted successfully!`),
      error: (err: string) => this.notificationService.error(err),
      complete: () => void this.router.navigate(['/clients']),
    };

    this.client$
      .pipe(
        take(1),
        switchMap((client) => this.clientService.delete(client.id)),
      )
      .subscribe(deleteObserver);
  }
}
