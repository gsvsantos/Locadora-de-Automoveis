import { AuthService } from './../../../services/auth.service';
import { AdminService } from './../../../services/admin.service';
import { Component, inject } from '@angular/core';
import { Tenant } from '../../../models/admin.models';
import { AsyncPipe, SlicePipe, TitleCasePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { catchError, EMPTY, exhaustMap, filter, map, merge, startWith, Subject, tap } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-list-tenants.component',
  imports: [AsyncPipe, ClipboardModule, TitleCasePipe, TranslocoModule, SlicePipe, FormsModule],
  templateUrl: './list-tenants.component.html',
  styleUrl: './list-tenants.component.scss',
})
export class ListTenantsComponent {
  protected readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly adminService: AdminService = inject(AdminService);
  protected readonly authService: AuthService = inject(AuthService);
  protected readonly notificationService = inject(NotificationService);

  private readonly impersonateClickSubject$ = new Subject<string>();
  private readonly deimpersonateClickSubject$ = new Subject<void>();

  protected readonly tenants$ = this.route.data.pipe(
    filter((data) => data['tenants'] as boolean),
    map((data) => data['tenants'] as Tenant[]),
  );

  protected readonly isImpersonating$ = this.authService
    .getAuthMode()
    .pipe(map((mode) => mode === 'impersonated'));

  protected readonly effects$ = merge(
    this.impersonateClickSubject$.pipe(
      exhaustMap((tenantId: string) =>
        this.authService.startImpersonation(tenantId).pipe(
          tap(() => {
            this.notificationService.success('Impersonation enabled.');
            void this.router.navigate(['/home']);
          }),
          catchError((err: string) => {
            this.notificationService.error(String(err));
            return EMPTY;
          }),
        ),
      ),
    ),

    this.deimpersonateClickSubject$.pipe(
      exhaustMap(() =>
        this.authService.stopImpersonation().pipe(
          tap(() => {
            this.notificationService.success('Back to PlatformAdmin.');
            void this.router.navigate(['/admin/tenants']);
          }),
          catchError((err: string) => {
            this.notificationService.error(String(err));
            return EMPTY;
          }),
        ),
      ),
    ),
  ).pipe(
    map(() => true),
    startWith(true),
  );

  protected impersonate(tenantId: string): void {
    this.impersonateClickSubject$.next(tenantId);
  }

  protected deimpersonate(): void {
    this.deimpersonateClickSubject$.next();
  }

  protected message(success: boolean): void {
    if (success) this.notificationService.success('ID copied to clipboard!');
    else this.notificationService.error('Copy failed!');
  }
}
