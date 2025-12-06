import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  BehaviorSubject,
  catchError,
  defer,
  distinctUntilChanged,
  Observable,
  of,
  shareReplay,
  switchMap,
  tap,
} from 'rxjs';
import { AuthApiResponse, LoginAuthDto, RegisterAuthDto } from '../models/auth.models';

@Injectable()
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl + '/auth';

  private readonly accessTokenSubject$ = new BehaviorSubject<AuthApiResponse | undefined>(
    undefined,
  );

  private readonly init$ = defer(() => this.refresh().pipe(catchError((err) => of(err)))).pipe(
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  public getAccessToken(): Observable<AuthApiResponse | undefined> {
    return this.init$.pipe(
      switchMap(() => this.accessTokenSubject$.asObservable()),
      distinctUntilChanged((first, second) => first?.key === second?.key),
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

    return this.http
      .post<AuthApiResponse>(url, {})
      .pipe(tap((token) => this.accessTokenSubject$.next(token)));
  }

  public logout(): Observable<null> {
    const urlCompleto = `${this.apiUrl}/logout`;

    return this.http.post<null>(urlCompleto, {}).pipe(tap(() => this.revokeAccessToken()));
  }

  public revokeAccessToken(): void {
    return this.accessTokenSubject$.next(undefined);
  }
}
