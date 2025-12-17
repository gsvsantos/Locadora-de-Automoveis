import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { map, Observable, take } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authenticatedUserGuard: CanActivateFn = (): Observable<true | UrlTree> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getAccessToken().pipe(
    take(1),
    map((token) => {
      if (!token) return router.createUrlTree(['/auth/login']);

      const isClient = token.user.roles.includes('Client');

      if (isClient) {
        return router.createUrlTree(['/auth/login']);
      }

      const isPlatformAdmin = token.user.roles.includes('PlatformAdmin');

      if (isPlatformAdmin) {
        return router.createUrlTree(['/admin/tenants']);
      }

      return true;
    }),
  );
};
