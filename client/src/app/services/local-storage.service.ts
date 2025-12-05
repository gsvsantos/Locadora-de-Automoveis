import { Injectable } from '@angular/core';
import { AuthApiResponse } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class LocalStorageService {
  private readonly accessTokenKey: string = 'locadoradeautomoveis:access-token';

  public saveAccessToken(token: AuthApiResponse): void {
    const jsonString = JSON.stringify(token);

    localStorage.setItem(this.accessTokenKey, jsonString);
  }

  public clearAccessToken(): void {
    localStorage.removeItem(this.accessTokenKey);
  }

  public getAccessToken(): AuthApiResponse | undefined {
    const jsonString = localStorage.getItem(this.accessTokenKey);

    if (!jsonString) return undefined;

    return JSON.parse(jsonString) as AuthApiResponse;
  }
}
