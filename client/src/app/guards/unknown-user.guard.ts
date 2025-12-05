import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { map, Observable, take } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const unknownUserGuard: CanActivateFn = (): Observable<true | UrlTree> => {
  const authService = inject(AuthService);
  const router: Router = inject(Router);

  return authService.accessToken$.pipe(
    take(1),
    map((token) => (!token ? true : router.createUrlTree(['/auth/register']))),
  );
};
