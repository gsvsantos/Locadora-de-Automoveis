import { Routes } from '@angular/router';
import { ListVehiclesComponent } from '../components/vehicles/list/list-vehicles.component';
import { listVehiclesResolver, vehicleDetailsResolver } from '../resolvers/vehicle.resolvers';
import { CreateVehicleComponent } from '../components/vehicles/create/create-vehicle.component';
import { listGroupsResolver } from '../resolvers/group.resolvers';
import { UpdateVehicleComponent } from '../components/vehicles/update/update-vehicle.component';

export const vehicleRoutes: Routes = [
  {
    path: '',
    component: ListVehiclesComponent,
    resolve: { vehicles: listVehiclesResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'register',
    component: CreateVehicleComponent,
    resolve: { groups: listGroupsResolver, vehicles: listVehiclesResolver },
  },
  {
    path: 'edit/:id',
    component: UpdateVehicleComponent,
    resolve: { groups: listGroupsResolver, vehicle: vehicleDetailsResolver },
  },
  //   {
  //     path: 'delete/:id',
  //     component: Delete, groups: listGroupsResolverComponent,
  //     resolve: { vehicle: vehicleDetailsResolver },
  //   },
];
