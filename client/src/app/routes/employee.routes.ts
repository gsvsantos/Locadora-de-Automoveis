import { Routes } from '@angular/router';
import { ListEmployeesComponent } from '../components/employees/list-employees/list-employees.component';
import { listEmployeesResolver } from '../resolvers/employee.resolvers';

export const employeeRoutes: Routes = [
  {
    path: '',
    component: ListEmployeesComponent,
    resolve: { employees: listEmployeesResolver },
  },
];
