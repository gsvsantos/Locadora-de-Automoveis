import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Group } from '../models/group.models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private readonly apiUrl: string = environment.apiUrl + '/group';
  private readonly http: HttpClient = inject(HttpClient);

  public getAllDistinct(): Observable<Group[]> {
    return this.http.get<Group[]>(`${this.apiUrl}/groups`);
  }
}
