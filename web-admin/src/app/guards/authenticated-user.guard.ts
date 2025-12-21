import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { map, Observable, take } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

export const authenticatedUserGuard: CanActivateFn = (): Observable<true | UrlTree> => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const router = inject(Router);

  return authService.getAccessToken().pipe(
    take(1),
    map((token) => {
      if (!token) return router.createUrlTree(['/auth/login']);

      const roles = token.user?.roles ?? [];

      if (roles.includes('Client')) {
        authService.revokeAccessToken();
        notificationService.error('User not allowed');
        return router.createUrlTree(['/auth/login'], {
          queryParams: { reason: 'not-allowed' },
        });
      }

      if (roles.includes('PlatformAdmin')) {
        return router.createUrlTree(['/admin/tenants']);
      }

      return true;
    }),
  );
};
