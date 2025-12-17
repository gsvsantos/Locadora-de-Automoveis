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
            console.log('No active session found during init:', err);
            return of(undefined);
          }),
        ),
      );
    } catch (err) {
      console.log('Auth init failed silently', err);
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

  public forgotPassword(model: ForgotPasswordRequestDto): Observable<void> {
    const url = `${this.apiUrl}/forgot-password`;

    return this.http
      .post<void>(url, model)
      .pipe(tap(() => (this.revokeAccessToken(), this.oauthService.logOut())));
  }

  public resetPassword(model: ResetPasswordRequestDto): Observable<void> {
    const url = `${this.apiUrl}/reset-password`;

    return this.http
      .post<void>(url, model)
      .pipe(tap(() => (this.revokeAccessToken(), this.oauthService.logOut())));
  }

  public changePassword(model: ChangePasswordRequestDto): Observable<void> {
    const url = `${this.apiUrl}/change-password`;

    return this.http
      .post<void>(url, model)
      .pipe(tap(() => (this.revokeAccessToken(), this.oauthService.logOut())));
  }

  public logout(): Observable<null> {
    const url = `${this.apiUrl}/logout`;

    return this.http
      .post<null>(url, {})
      .pipe(tap(() => (this.revokeAccessToken(), this.oauthService.logOut())));
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
    const url = `${this.apiUrl}/google-login`;

    return this.http.post<AuthApiResponse>(url, { idToken });
  }
}
