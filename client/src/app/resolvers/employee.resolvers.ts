import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Employee } from '../models/employee.models';
import { EmployeeService } from '../services/employee.service';

export const listEmployeesResolver: ResolveFn<Employee[]> = () => {
  const employeeService = inject(EmployeeService);
  return employeeService.getAll();
};

export const employeeDetailsResolver: ResolveFn<Employee> = (route: ActivatedRouteSnapshot) => {
  const employeeService = inject(EmployeeService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;

  return employeeService.getById(id);
};
