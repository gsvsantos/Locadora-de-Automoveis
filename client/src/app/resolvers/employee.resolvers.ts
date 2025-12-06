import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Employee } from '../models/employee.models';
import { EmployeeService } from '../services/employee.service';
import { Observable } from 'rxjs';

export const listEmployeesResolver: ResolveFn<Employee[]> = (
  route: ActivatedRouteSnapshot,
): Observable<Employee[]> => {
  const employeeService: EmployeeService = inject(EmployeeService);

  const rawQuantity: string | null = route.queryParamMap.get('quantity');
  const rawIsActive: string | null = route.queryParamMap.get('isActive');

  const quantityFilter: number | undefined =
    rawQuantity !== null && rawQuantity.trim() !== '' ? Number(rawQuantity) : undefined;

  let isActiveFilter: boolean | undefined;

  switch (rawIsActive) {
    case 'true':
      isActiveFilter = true;
      break;
    case 'false':
      isActiveFilter = false;
      break;
    default:
      isActiveFilter = undefined;
      break;
  }

  return employeeService.getAll(quantityFilter, isActiveFilter);
};

export const employeeDetailsResolver: ResolveFn<Employee> = (route: ActivatedRouteSnapshot) => {
  const employeeService = inject(EmployeeService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;
  console.log('id no resolver', id);
  return employeeService.getById(id);
};
