import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Employee } from '../models/employee.models';
import { EmployeeService } from '../services/employee.service';

export const listEmployeesResolver: ResolveFn<Employee[]> = (route: ActivatedRouteSnapshot) => {
  const employeeService = inject(EmployeeService);

  const rawQuantity = route.queryParams['quantity'];
  const rawIsActive = route.queryParams['isActive'];

  const quantity = rawQuantity ? Number(rawQuantity) : undefined;
  let isActive: boolean | undefined;
  if (rawIsActive === 'true') {
    isActive = true;
  } else if (rawIsActive === 'false') {
    isActive = false;
  } else {
    isActive = undefined;
  }

  return employeeService.getAll(quantity, isActive);
};

export const employeeDetailsResolver: ResolveFn<Employee> = (route: ActivatedRouteSnapshot) => {
  const employeeService = inject(EmployeeService);

  if (!route.paramMap.has('id')) throw new Error('Missing "ID" parameter.');

  const id: string = route.paramMap.get('id')!;
  console.log('id no resolver', id);
  return employeeService.getById(id);
};
