import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Group, GroupDto, ListGroupsDto } from '../models/group.models';
import { Observable, map } from 'rxjs';
import { ApiResponseDto, IdApiResponse } from '../models/api.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private readonly apiUrl: string = environment.apiUrl + '/group';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: GroupDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Group[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListGroupsDto>),
      map((res) => res.groups),
    );
  }
}
