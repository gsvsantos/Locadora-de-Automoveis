import { HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, Observable, switchMap, take } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';
import { mapApiErroResponse } from '../utils/map-api-response';

export const authInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const router = inject(Router);

  const whitelist = [
    '/auth/register',
    '/auth/login',
    '/auth/refresh',
    '/auth/logout',
    '/auth/reset-password',
  ];

  if (whitelist.some((url) => req.url.includes(url))) {
    return next(req.clone({ withCredentials: true })).pipe(
      catchError(
        (err: HttpErrorResponse) => mapApiErroResponse(err) as Observable<HttpEvent<unknown>>,
      ),
    );
  }

  return authService.getAccessToken().pipe(
    take(1),
    switchMap((accessToken) => {
      if (!accessToken) return next(req.clone({ withCredentials: true }));

      const request = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${accessToken.key}`),
        withCredentials: true,
      });

      return next(request).pipe(
        catchError((err: HttpErrorResponse) => {
          if (err.status !== 401) return mapApiErroResponse(err) as Observable<HttpEvent<unknown>>;

          return authService.refresh().pipe(
            switchMap((newAccessToken) => {
              const newRequest = req.clone({
                headers: req.headers.set('Authorization', `Bearer ${newAccessToken.key}`),
                withCredentials: true,
              });

              return next(newRequest);
            }),
            catchError((refreshError: HttpErrorResponse) => {
              notificationService.error('Your session has expired. Please, log-in again.');

              authService.revokeAccessToken();

              void router.navigate(['/auth', 'login']);

              return mapApiErroResponse(refreshError) as Observable<HttpEvent<unknown>>;
            }),
          );
        }),
      );
    }),
  );
};
