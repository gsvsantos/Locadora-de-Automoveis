import { LocalStorageService } from './local-storage.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
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
import {
  AuthApiResponse,
  AuthMode,
  ChangePasswordRequestDto,
  ForgotPasswordRequestDto,
  LoginAuthDto,
  RegisterAuthDto,
  ResetPasswordRequestDto,
} from '../models/auth.models';
import { OAuthService } from 'angular-oauth2-oidc';
import { googleAuthConfig } from '../core/auth.google.config';
import { NotificationService } from './notification.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly localStorageService = inject(LocalStorageService);
  private readonly notificationService = inject(NotificationService);
  private readonly oauthService = inject(OAuthService);
  private readonly http: HttpClient = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl: string = environment.apiUrl + '/auth';

  private readonly authModeSubject$ = new BehaviorSubject<AuthMode>('platform');

  private readonly accessTokenSubject$ = new BehaviorSubject<AuthApiResponse | undefined>(
    undefined,
  );

  public constructor() {
    this.oauthService.configure(googleAuthConfig);
    this.oauthService.setupAutomaticSilentRefresh();
  }

  public async initialize(): Promise<void> {
    const isLoggedFlag = this.localStorageService.getLogged();
    const comingFromGoogle = this.isComingFromGoogle;

    if (!isLoggedFlag && !comingFromGoogle) {
      return;
    }

    try {
      await firstValueFrom(
        from(this.oauthService.tryLogin()).pipe(
          switchMap(() => {
            if (this.oauthService.hasValidIdToken() && comingFromGoogle) {
              const googleIdToken = this.oauthService.getIdToken();
              return this.loginWithGoogleBackend(googleIdToken);
            }

            return this.refresh();
          }),
          tap((token) => {
            if (token) {
              this.handleLoginSuccess(token);

              if (comingFromGoogle) {
                this.clearUrlParams();
                this.oauthService.logOut();
              }
            }
          }),
          catchError((err: HttpErrorResponse) => {
            console.log('Auth failed:', err);

            if (this.oauthService.hasValidIdToken()) {
              const apiError = err.error as { errors?: string[] };
              const errorMessage: string | undefined = apiError?.errors?.[0];
              this.notificationService.error(errorMessage!);
              this.clearUrlParams();
            }
            this.handleLogoutCleanup();
            return of(undefined);
          }),
        ),
      );
    } catch (err) {
      console.log('Auth init failed silently', err);
      this.handleLogoutCleanup();
    }
  }

  public hasRole(role: string): boolean {
    const user = this.accessTokenSubject$.value?.user;
    if (!user || !user.roles) return false;
    return user.roles.includes(role);
  }

  public get isClient(): boolean {
    return this.hasRole('Client');
  }

  public getAccessToken(): Observable<AuthApiResponse | undefined> {
    return this.accessTokenSubject$.asObservable();
  }

  public getAuthMode(): Observable<AuthMode> {
    return this.authModeSubject$.asObservable();
  }

  public register(model: RegisterAuthDto): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/register-client`;

    return this.http
      .post<AuthApiResponse>(url, model)
      .pipe(tap((token) => this.handleLoginSuccess(token)));
  }

  public login(loginModel: LoginAuthDto): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/login`;

    return this.http
      .post<AuthApiResponse>(url, loginModel)
      .pipe(tap((token) => this.handleLoginSuccess(token)));
  }

  public refresh(): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/refresh`;

    return this.http.post<AuthApiResponse>(url, {}, { withCredentials: true }).pipe(
      tap((token: AuthApiResponse) => {
        this.authModeSubject$.next('platform');
        this.handleLoginSuccess(token);
      }),
    );
  }

  public logout(): Observable<null> {
    const url = `${this.apiUrl}/logout`;

    return this.http.post<null>(url, {}).pipe(
      tap(() => {
        this.handleLogoutCleanup();
        this.oauthService.logOut();
      }),
    );
  }

  public forgotPassword(model: ForgotPasswordRequestDto): Observable<void> {
    const url = `${this.apiUrl}/forgot-password`;

    return this.http
      .post<void>(url, model)
      .pipe(tap(() => (this.handleLogoutCleanup(), this.oauthService.logOut())));
  }

  public resetPassword(model: ResetPasswordRequestDto): Observable<void> {
    const url = `${this.apiUrl}/reset-password`;

    return this.http
      .post<void>(url, model)
      .pipe(tap(() => (this.handleLogoutCleanup(), this.oauthService.logOut())));
  }

  public changePassword(model: ChangePasswordRequestDto): Observable<void> {
    const url = `${this.apiUrl}/change-password`;

    return this.http
      .post<void>(url, model)
      .pipe(tap(() => (this.handleLogoutCleanup(), this.oauthService.logOut())));
  }

  public revokeAccessToken(): void {
    this.authModeSubject$.next('platform');

    return this.accessTokenSubject$.next(undefined);
  }

  public loginWithGoogle(): void {
    this.oauthService.initLoginFlow();
  }

  private loginWithGoogleBackend(idToken: string): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/login-google-client`;

    return this.http.post<AuthApiResponse>(url, { idToken });
  }

  private handleLoginSuccess(token: AuthApiResponse): void {
    this.accessTokenSubject$.next(token);
    this.localStorageService.setLogged();
  }

  private handleLogoutCleanup(): void {
    this.revokeAccessToken();
    this.localStorageService.removeLogged();
  }

  private get isComingFromGoogle(): boolean {
    const url = window.location.href;
    return url.includes('code=') || url.includes('id_token=') || url.includes('access_token=');
  }

  private clearUrlParams(): void {
    void this.router.navigate([], {
      queryParams: {},
      fragment: undefined,
      replaceUrl: true,
    });
  }
}
