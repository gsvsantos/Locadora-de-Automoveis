import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { map, Observable, take } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const unknownUserGuard: CanActivateFn = (): Observable<true | UrlTree> => {
  const authService = inject(AuthService);
  const router: Router = inject(Router);

  return authService.getAccessToken().pipe(
    take(1),
    map(
      (token) => (
        console.log('unknownUserGuard', token),
        !token ? true : router.createUrlTree(['/home'])
      ),
    ),
  );
};
