import { Routes } from '@angular/router';
import { ListPartnersComponent } from '../components/partners/list/list-partners.component';
import { listPartnersResolver, partnerDetailsResolver } from '../resolvers/partner.resolvers';
import { CreatePartnerComponent } from '../components/partners/create/create-partner.component';
import { DeletePartnerComponent } from '../components/partners/delete/delete-partner.component';
import { UpdatePartnerComponent } from '../components/partners/update/update-partner.component';
import { GetCouponsPartnerComponent } from '../components/partners/get-coupons/get-coupons-partner.component';

export const partnerRoutes: Routes = [
  {
    path: '',
    component: ListPartnersComponent,
    resolve: { partners: listPartnersResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  { path: 'register', component: CreatePartnerComponent },
  {
    path: 'edit/:id',
    component: UpdatePartnerComponent,
    resolve: { partner: partnerDetailsResolver },
  },
  {
    path: 'delete/:id',
    component: DeletePartnerComponent,
    resolve: { partner: partnerDetailsResolver },
  },
  {
    path: 'partner/:id/coupons',
    component: GetCouponsPartnerComponent,
    resolve: { partner: partnerDetailsResolver },
  },
];
