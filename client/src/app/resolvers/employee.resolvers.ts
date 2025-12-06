import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { Employee } from '../models/employee.models';
import { EmployeeService } from '../services/employee.service';

export const listEmployeesResolver: ResolveFn<Employee[]> = () => {
  const employeeService = inject(EmployeeService);
  return employeeService.getAll();
};
