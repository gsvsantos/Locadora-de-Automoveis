import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  BehaviorSubject,
  catchError,
  defer,
  distinctUntilChanged,
  finalize,
  from,
  Observable,
  of,
  shareReplay,
  switchMap,
  tap,
} from 'rxjs';
import { AuthApiResponse, LoginAuthDto, RegisterAuthDto } from '../models/auth.models';
import { OAuthService } from 'angular-oauth2-oidc';
import { googleAuthConfig } from '../core/auth.google.config';
import { Router } from '@angular/router';

@Injectable()
export class AuthService {
  private oauthService = inject(OAuthService);
  private readonly http: HttpClient = inject(HttpClient);
  private readonly router: Router = inject(Router);
  private readonly apiUrl: string = environment.apiUrl + '/auth';

  private readonly accessTokenSubject$ = new BehaviorSubject<AuthApiResponse | undefined>(
    undefined,
  );

  public constructor() {
    this.oauthService.configure(googleAuthConfig);
    this.oauthService.setupAutomaticSilentRefresh();
  }

  private readonly init$ = defer(() =>
    from(this.oauthService.tryLogin()).pipe(
      switchMap(() => {
        const hasValidToken = this.oauthService.hasValidIdToken();

        if (hasValidToken) {
          const googleIdToken = this.oauthService.getIdToken();
          return this.loginWithGoogleBackend(googleIdToken).pipe(
            finalize(() => void this.router.navigate(['/home'])),
            catchError(() => of(undefined)),
          );
        }

        return this.refresh().pipe(
          finalize(() => void this.router.navigate(['/home'])),
          catchError(() => of(undefined)),
        );
      }),
      tap((token) => {
        this.accessTokenSubject$.next(token);
      }),
      catchError(() => of(undefined)),
    ),
  ).pipe(shareReplay({ bufferSize: 1, refCount: true }));

  public getAccessToken(): Observable<AuthApiResponse | undefined> {
    this.init$.subscribe();

    return this.accessTokenSubject$
      .asObservable()
      .pipe(distinctUntilChanged((first, second) => first?.key === second?.key));
  }

  private loginWithGoogleBackend(idToken: string): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/google-login`;

    return this.http.post<AuthApiResponse>(url, { idToken }).pipe(
      tap((token) => {
        this.accessTokenSubject$.next(token);
      }),
    );
  }

  public loginWithGoogle(): void {
    this.oauthService.initLoginFlow();
  }

  public register(model: RegisterAuthDto): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/register`;

    return this.http
      .post<AuthApiResponse>(url, model)
      .pipe(tap((token) => this.accessTokenSubject$.next(token)));
  }

  public login(loginModel: LoginAuthDto): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/login`;

    return this.http
      .post<AuthApiResponse>(url, loginModel)
      .pipe(tap((token) => this.accessTokenSubject$.next(token)));
  }

  public refresh(): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/refresh`;

    return this.http
      .post<AuthApiResponse>(url, {})
      .pipe(tap((token) => this.accessTokenSubject$.next(token)));
  }

  public logout(): Observable<null> {
    const urlCompleto = `${this.apiUrl}/logout`;

    return this.http
      .post<null>(urlCompleto, {})
      .pipe(tap(() => (this.revokeAccessToken(), this.oauthService.logOut())));
  }

  public revokeAccessToken(): void {
    return this.accessTokenSubject$.next(undefined);
  }
}
