import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { mapApiResponse } from '../utils/map-api-response';
import { ApiResponseDto } from '../models/api.models';
import { GlobalSearch, ListGlobalSearchDto } from '../models/search.models';

@Injectable({
  providedIn: 'root',
})
export class SearchService {
  private readonly apiUrl: string = environment.apiUrl + '/search';
  private readonly http: HttpClient = inject(HttpClient);

  public globalSearch(term: string): Observable<GlobalSearch[]> {
    const params = new HttpParams().set('term', term);

    return this.http.get<ApiResponseDto>(this.apiUrl, { params: params }).pipe(
      map(mapApiResponse<ListGlobalSearchDto>),
      map((res) => res.items),
    );
  }
}
