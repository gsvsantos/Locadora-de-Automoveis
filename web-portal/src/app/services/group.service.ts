import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Group, ListGroupsDto } from '../models/group.models';
import { map, Observable } from 'rxjs';
import { mapApiResponse } from '../utils/map-api-response';
import { ApiResponseDto } from '../models/api.models';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private readonly apiUrl: string = environment.apiUrl + '/group';
  private readonly http: HttpClient = inject(HttpClient);

  public getAllDistinct(): Observable<Group[]> {
    const url = `${this.apiUrl}/groups`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ListGroupsDto>),
      map((res) => res.groups),
    );
  }
}
