import { HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';
import { LocalStorageService } from '../services/local-storage.service';
import { mapApiErroResponse } from '../utils/map-api-response';

export const authInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const localStorageService = inject(LocalStorageService);
  const router = inject(Router);

  const accessToken = authService.accessTokenSubject$.getValue();
  const bearer = accessToken?.key ?? localStorageService.getAccessToken()?.key;

  if (bearer) {
    const requisicaoClonada = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${bearer}`),
    });

    return next(requisicaoClonada).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status === 401) {
          notificationService.error('Session expired. Please log in again');
          authService.accessTokenSubject$.next(undefined);
          void router.navigate(['/auth', 'login']);
        }

        return mapApiErroResponse(err) as Observable<HttpEvent<unknown>>;
      }),
    );
  }

  return next(req).pipe(
    catchError(
      (err: HttpErrorResponse) => mapApiErroResponse(err) as Observable<HttpEvent<unknown>>,
    ),
  );
};
