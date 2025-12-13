import { Routes } from '@angular/router';
import { ListEmployeesComponent } from '../components/employees/list/list-employees.component';
import { employeeDetailsResolver, listEmployeesResolver } from '../resolvers/employee.resolvers';
import { CreateEmployeeComponent } from '../components/employees/create/create-employee.component';
import { UpdateEmployeeComponent } from '../components/employees/update/update-employee.component';
import { DeleteEmployeeComponent } from '../components/employees/delete/delete-employee.component';
import { SelfUpdateEmployeeComponent } from '../components/employees/self-update/self-update-employee.component';
import { adminOnlyGuard } from '../guards/admin-only.guard';
import { employeeOnlyGuard } from '../guards/employee-only.guard';

export const employeeRoutes: Routes = [
  {
    path: '',
    component: ListEmployeesComponent,
    resolve: { employees: listEmployeesResolver },
    canActivate: [adminOnlyGuard],
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  { path: 'register', component: CreateEmployeeComponent, canActivate: [adminOnlyGuard] },
  {
    path: 'edit/:id',
    component: UpdateEmployeeComponent,
    resolve: { employee: employeeDetailsResolver },
    canActivate: [adminOnlyGuard],
  },
  {
    path: 'options/:id',
    component: SelfUpdateEmployeeComponent,
    canActivate: [employeeOnlyGuard],
  },
  {
    path: 'delete/:id',
    component: DeleteEmployeeComponent,
    resolve: { employee: employeeDetailsResolver },
    canActivate: [adminOnlyGuard],
  },
];
