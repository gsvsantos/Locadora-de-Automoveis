import { Routes } from '@angular/router';
import { ListExtrasComponent } from '../components/extras/list/list-extras.component';
import { listExtrasResolver } from '../resolvers/extra.resolvers';

export const extraRoutes: Routes = [
  {
    path: '',
    component: ListExtrasComponent,
    resolve: { extras: listExtrasResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  //   {
  //     path: 'register',
  //     component: CreateExtraComponent,
  //   },
  //   {
  //     path: 'edit/:id',
  //     component: UpdateExtraComponent,
  //     resolve: { extra: extraDetailsResolver },
  //   },
  //   {
  //     path: 'delete/:id',
  //     component: DeleteExtraComponent,
  //     resolve: { extra: extraDetailsResolver },
  //   },
];
