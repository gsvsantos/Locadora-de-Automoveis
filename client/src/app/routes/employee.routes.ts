import { Routes } from '@angular/router';
import { ListEmployeesComponent } from '../components/employees/list-employees/list-employees.component';
import { listEmployeesResolver } from '../resolvers/employee.resolvers';
import { CreateEmployeeComponent } from '../components/employees/create-employee/create-employee.component';

export const employeeRoutes: Routes = [
  {
    path: '',
    component: ListEmployeesComponent,
    resolve: { employees: listEmployeesResolver },
  },
  { path: 'register', component: CreateEmployeeComponent },
];
