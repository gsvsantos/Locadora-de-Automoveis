import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

export const EmployeeOnlyGuard: CanActivateFn = (): Observable<boolean> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getAccessToken().pipe(
    map((accessToken) => accessToken?.user),

    map((user) => {
      const isEmployee = user?.role?.toLowerCase() === 'employee';

      if (isEmployee) {
        return true;
      } else {
        void router.navigate(['/employees']);

        return false;
      }
    }),

    take(1),
  );
};
