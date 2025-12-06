import { Routes } from '@angular/router';
import { ListEmployeesComponent } from '../components/employees/list/list-employees.component';
import { employeeDetailsResolver, listEmployeesResolver } from '../resolvers/employee.resolvers';
import { CreateEmployeeComponent } from '../components/employees/create/create-employee.component';
import { UpdateEmployeeComponent } from '../components/employees/update/update-employee.component';
import { DeleteEmployeeComponent } from '../components/employees/delete/delete-employee.component';
import { SelfUpdateEmployeeComponent } from '../components/employees/self-update/self-update-employee.component';
import { AdminOnlyGuard } from '../guards/admin-only.guard';
import { EmployeeOnlyGuard } from '../guards/employee-only.guard';

export const employeeRoutes: Routes = [
  {
    path: '',
    component: ListEmployeesComponent,
    resolve: { employees: listEmployeesResolver },
    canActivate: [AdminOnlyGuard],
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  { path: 'register', component: CreateEmployeeComponent, canActivate: [AdminOnlyGuard] },
  {
    path: 'edit/:id',
    component: UpdateEmployeeComponent,
    resolve: { employee: employeeDetailsResolver },
    canActivate: [AdminOnlyGuard],
  },
  {
    path: 'options/:id',
    component: SelfUpdateEmployeeComponent,
    canActivate: [EmployeeOnlyGuard],
  },
  {
    path: 'delete/:id',
    component: DeleteEmployeeComponent,
    resolve: { employee: employeeDetailsResolver },
    canActivate: [AdminOnlyGuard],
  },
];
