import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {
  Employee,
  EmployeeDetailsApiDto,
  EmployeeDto,
  ListEmployeesDto,
} from '../models/employee.models';
import { ApiResponseDto, IdApiResponse } from '../models/api.models';
import { Observable, map, tap } from 'rxjs';
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

  public update(id: string, updateModel: EmployeeDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public getById(id: string): Observable<Employee> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<EmployeeDetailsApiDto>),
      map((apiDto: EmployeeDetailsApiDto) => this.mapEmployeeFromApi(apiDto.employee)),
    );
  }

  public getAll(): Observable<Employee[]> {
    const url = `${this.apiUrl}/get-all`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ListEmployeesDto>),
      map((res) => res.employees),
    );
  }

  private mapEmployeeFromApi(apiEmployee: EmployeeDetailsApiDto['employee']): Employee {
    return {
      id: apiEmployee.id,
      fullName: apiEmployee.fullName,
      admissionDate: new Date(apiEmployee.admissionDate),
      salary: apiEmployee.salary,
    };
  }
}
