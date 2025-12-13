import { Component, inject } from '@angular/core';
import { Tenant } from '../../../models/admin.models';
import { AsyncPipe, SlicePipe, TitleCasePipe } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { filter, map } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-list-tenants.component',
  imports: [
    AsyncPipe,
    ClipboardModule,
    TitleCasePipe,
    TranslocoModule,
    RouterLink,
    SlicePipe,
    FormsModule,
  ],
  templateUrl: './list-tenants.component.html',
  styleUrl: './list-tenants.component.scss',
})
export class ListTenantsComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly notificationService = inject(NotificationService);

  protected readonly tenants$ = this.route.data.pipe(
    filter((data) => data['tenants'] as boolean),
    map((data) => data['tenants'] as Tenant[]),
  );

  protected message(success: boolean): void {
    if (success) {
      this.notificationService.success(`ID copied to clipboard!`);
    } else {
      this.notificationService.error(`Copy failed!`);
    }
  }
}
