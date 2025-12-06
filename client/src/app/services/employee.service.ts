import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Employee, EmployeeDto, ListEmployeesDto } from '../models/employee.models';
import { ApiResponseDto, IdApiResponse } from '../models/api.models';
import { Observable, map } from 'rxjs';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private readonly apiUrl: string = environment.apiUrl + '/employee';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: EmployeeDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public getById(id: string): Observable<Employee> {
    const url = `${this.apiUrl}/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(map(mapApiResponse<Employee>));
  }

  public getAll(): Observable<Employee[]> {
    const url = `${this.apiUrl}/get-all`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ListEmployeesDto>),
      map((res) => res.employees),
    );
  }
}
