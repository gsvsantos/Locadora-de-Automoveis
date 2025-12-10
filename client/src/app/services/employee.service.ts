import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  Employee,
  EmployeeDetailsApiDto,
  EmployeeDto,
  ListEmployeesDto,
} from '../models/employee.models';
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

  public update(id: string, updateModel: EmployeeDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public selfUpdate(updateModel: EmployeeDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/self-update`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<Employee> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<EmployeeDetailsApiDto>),
      map((apiDto: EmployeeDetailsApiDto) => this.mapEmployeeFromApi(apiDto.employee)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<Employee[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
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
      isActive: apiEmployee.isActive,
    };
  }
}
