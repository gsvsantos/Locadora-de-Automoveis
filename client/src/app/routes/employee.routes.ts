import { Routes } from '@angular/router';
import { ListEmployeesComponent } from '../components/employees/list/list-employees.component';
import { employeeDetailsResolver, listEmployeesResolver } from '../resolvers/employee.resolvers';
import { CreateEmployeeComponent } from '../components/employees/create/create-employee.component';
import { UpdateEmployeeComponent } from '../components/employees/update/update-employee.component';

export const employeeRoutes: Routes = [
  {
    path: '',
    component: ListEmployeesComponent,
    resolve: { employees: listEmployeesResolver },
  },
  { path: 'register', component: CreateEmployeeComponent },
  {
    path: 'edit/:id',
    component: UpdateEmployeeComponent,
    resolve: { employee: employeeDetailsResolver },
  },
];
