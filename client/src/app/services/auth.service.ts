import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  BehaviorSubject,
  catchError,
  firstValueFrom,
  from,
  Observable,
  of,
  switchMap,
  tap,
} from 'rxjs';
import { AuthApiResponse, AuthMode, LoginAuthDto, RegisterAuthDto } from '../models/auth.models';
import { OAuthService } from 'angular-oauth2-oidc';
import { googleAuthConfig } from '../core/auth.google.config';
import { AdminService } from './admin.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly oauthService = inject(OAuthService);
  private readonly adminService = inject(AdminService);
  private readonly http: HttpClient = inject(HttpClient);
  private readonly apiUrl: string = environment.apiUrl + '/auth';

  private readonly authModeSubject$ = new BehaviorSubject<AuthMode>('platform');
  private platformAccessTokenSnapshot?: AuthApiResponse;

  private readonly accessTokenSubject$ = new BehaviorSubject<AuthApiResponse | undefined>(
    undefined,
  );

  public constructor() {
    this.oauthService.configure(googleAuthConfig);
    this.oauthService.setupAutomaticSilentRefresh();
  }

  public async initialize(): Promise<void> {
    try {
      await firstValueFrom(
        from(this.oauthService.tryLogin()).pipe(
          switchMap(() => {
            if (this.oauthService.hasValidIdToken()) {
              const googleIdToken = this.oauthService.getIdToken();
              return this.loginWithGoogleBackend(googleIdToken);
            }

            return this.refresh();
          }),
          tap((token) => {
            if (token) {
              this.accessTokenSubject$.next(token);
            }
          }),
          catchError((err) => {
            console.debug('No active session found during init:', err);
            return of(undefined);
          }),
        ),
      );
    } catch (err) {
      console.debug('Auth init failed silently', err);
    }
  }

  public getAccessToken(): Observable<AuthApiResponse | undefined> {
    return this.accessTokenSubject$.asObservable();
  }

  public getAuthMode(): Observable<AuthMode> {
    return this.authModeSubject$.asObservable();
  }

  public isImpersonatingNow(): boolean {
    return this.authModeSubject$.value === 'impersonated';
  }

  public startImpersonation(tenantId: string): Observable<AuthApiResponse> {
    const currentToken: AuthApiResponse | undefined = this.accessTokenSubject$.value;

    if (currentToken && !this.isImpersonatingNow()) {
      this.platformAccessTokenSnapshot = currentToken;
    }

    return this.adminService.impersonateTenant(tenantId).pipe(
      tap((impersonatedToken: AuthApiResponse) => {
        this.authModeSubject$.next('impersonated');
        this.accessTokenSubject$.next(impersonatedToken);
      }),
    );
  }

  public stopImpersonation(): Observable<AuthApiResponse> {
    return this.refresh();
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

    return this.http.post<AuthApiResponse>(url, {}, { withCredentials: true }).pipe(
      tap((token: AuthApiResponse) => {
        this.platformAccessTokenSnapshot = token;
        this.authModeSubject$.next('platform');
        this.accessTokenSubject$.next(token);
      }),
    );
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

  public loginWithGoogle(): void {
    this.oauthService.initLoginFlow();
  }

  private loginWithGoogleBackend(idToken: string): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/google-login`;

    return this.http.post<AuthApiResponse>(url, { idToken });
  }
}
