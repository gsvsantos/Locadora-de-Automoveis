import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  BehaviorSubject,
  defer,
  distinctUntilChanged,
  map,
  merge,
  Observable,
  of,
  shareReplay,
  skip,
  tap,
} from 'rxjs';
import { AuthApiResponse, LoginAuthDto, RegisterAuthDto } from '../models/auth.models';
import { LocalStorageService } from './local-storage.service';

@Injectable()
export class AuthService {
  private readonly localStorage = inject(LocalStorageService);
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl + '/auth';

  public readonly accessTokenSubject$ = new BehaviorSubject<AuthApiResponse | undefined>(undefined);

  public readonly accessTokenFromApi$: Observable<AuthApiResponse | undefined> =
    this.accessTokenSubject$.pipe(
      skip(1),
      map((apiResponse) => (apiResponse ? this.mapAccessToken(apiResponse) : undefined)),
    );

  public readonly storedAccessToken$: Observable<AuthApiResponse | undefined> = defer(() => {
    const accessToken = this.localStorage.getAccessToken();

    if (!accessToken) return of(undefined);

    const isValid = new Date(accessToken.expiration) > new Date();

    if (!isValid) return of(undefined);

    return of(accessToken);
  });

  public readonly accessToken$: Observable<AuthApiResponse | undefined> = merge(
    this.storedAccessToken$,
    this.accessTokenFromApi$,
  ).pipe(
    distinctUntilChanged((prev, curr) => prev === curr),
    tap((accessToken) => {
      if (accessToken) this.localStorage.saveAccessToken(accessToken);
      else this.localStorage.clearAccessToken();
    }),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

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

  public logout(): Observable<null> {
    const urlCompleto = `${this.apiUrl}/logout`;

    return this.http
      .post<null>(urlCompleto, {})
      .pipe(tap(() => this.accessTokenSubject$.next(undefined)));
  }

  private mapAccessToken(response: AuthApiResponse): AuthApiResponse {
    if (!response.key || !response.expiration || !response.user) {
      throw new Error('Something went wrong');
    }

    const { key, expiration, user: authenticatedUser } = response;

    return {
      key: key,
      expiration: new Date(expiration),
      user: authenticatedUser,
    };
  }
}
