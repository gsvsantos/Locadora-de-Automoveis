import { UpdateLanguageDto } from './../models/account.models';
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { LanguageCode } from './local-storage.service';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/account`;

  public setActiveLang(newLanguageCode: LanguageCode): Observable<null> {
    const url = `${this.apiUrl}/language`;

    const body: UpdateLanguageDto = { language: newLanguageCode };

    return this.http.put<null>(url, body);
  }
}
