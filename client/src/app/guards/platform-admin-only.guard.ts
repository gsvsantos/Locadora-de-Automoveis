import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map, Observable, take } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const platformAdminOnlyGuard: CanActivateFn = (): Observable<boolean> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getAccessToken().pipe(
    map((accessToken) => accessToken?.user),

    map((user) => {
      const isAdmin = user?.roles.find((role) => role === 'PlatformAdmin');

      if (isAdmin && user?.roles.includes('PlatformAdmin')) {
        return true;
      } else {
        void router.navigate(['/home']);
        return false;
      }
    }),

    take(1),
  );
};
