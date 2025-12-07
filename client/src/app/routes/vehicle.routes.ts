import { Routes } from '@angular/router';
import { ListVehiclesComponent } from '../components/vehicles/list/list-vehicles.component';
import { listVehiclesResolver } from '../resolvers/vehicle.resolvers';

export const vehicleRoutes: Routes = [
  {
    path: '',
    component: ListVehiclesComponent,
    resolve: { vehicles: listVehiclesResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  //   {
  //     path: 'register',
  //     component: CreateVehicleComponent,
  //     resolve: { vehicles: listVehiclesResolver },
  //   },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateVehicleComponent,
  //     resolve: { vehicle: vehicleDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: Delete, groups: listGroupsResolverComponent,
  //     resolve: { vehicle: vehicleDetailsResolver },
  //   },
];
