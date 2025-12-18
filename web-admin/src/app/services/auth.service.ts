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
  take,
  tap,
  throwError,
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
import { AdminService } from './admin.service';
import { LocalStorageService } from './local-storage.service';
import { NotificationService } from './notification.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly localStorageService = inject(LocalStorageService);
  private readonly notificationService = inject(NotificationService);
  private readonly oauthService = inject(OAuthService);
  private readonly adminService = inject(AdminService);
  private readonly http: HttpClient = inject(HttpClient);
  private readonly router = inject(Router);
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
          take(1),
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

  public getUserRole(): string {
    const user = this.accessTokenSubject$.value?.user;
    const roles = user?.roles ?? [];

    if (roles.includes('PlatformAdmin')) return 'platform-admin';
    if (roles.includes('Admin')) return 'admin';
    if (roles.includes('Employee')) return 'employee';

    return 'user';
  }

  public get isPlatformAdmin(): boolean {
    return this.hasRole('PlatformAdmin');
  }

  public get isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  public get isEmployee(): boolean {
    return this.hasRole('Employee');
  }

  public get isManager(): boolean {
    return this.isPlatformAdmin || this.isAdmin;
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
    return this.refresh().pipe(
      catchError((err: unknown) => {
        if (this.platformAccessTokenSnapshot) {
          this.authModeSubject$.next('platform');
          this.accessTokenSubject$.next(this.platformAccessTokenSnapshot);
          return of(this.platformAccessTokenSnapshot);
        }

        return throwError(() => err);
      }),
    );
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
        this.platformAccessTokenSnapshot = token;
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
    this.platformAccessTokenSnapshot = undefined;
    this.authModeSubject$.next('platform');
    return this.accessTokenSubject$.next(undefined);
  }

  public loginWithGoogle(): void {
    this.oauthService.initLoginFlow();
  }

  private loginWithGoogleBackend(idToken: string): Observable<AuthApiResponse> {
    const url = `${this.apiUrl}/login-google`;

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
