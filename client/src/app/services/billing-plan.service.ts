import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable, map } from 'rxjs';
import { ApiResponseDto, IdApiResponse } from '../models/api.models';
import { mapApiResponse } from '../utils/map-api-response';
import {
  BillingPlanDto,
  BillingPlan,
  BillingPlanDetailsApiDto,
  ListBillingPlansDto,
} from '../models/billing-plan.models';

@Injectable({
  providedIn: 'root',
})
export class BillingPlanService {
  private readonly apiUrl: string = environment.apiUrl + '/billing-plan';
  private readonly http: HttpClient = inject(HttpClient);

  public register(registerModel: BillingPlanDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/create`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }

  public update(id: string, updateModel: BillingPlanDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/update/${id}`;

    return this.http.put<IdApiResponse>(url, updateModel);
  }

  public delete(id: string): Observable<null> {
    const url = `${this.apiUrl}/delete/${id}`;

    return this.http.delete<null>(url);
  }

  public getById(id: string): Observable<BillingPlan> {
    const url: string = `${this.apiUrl}/get/${id}`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<BillingPlanDetailsApiDto>),
      map((apiDto: BillingPlanDetailsApiDto) => this.mapBillingPlanFromApi(apiDto.billingPlan)),
    );
  }

  public getAll(quantity?: number, isActive?: boolean): Observable<BillingPlan[]> {
    const url = `${this.apiUrl}/get-all`;
    let params = new HttpParams();

    if (quantity !== undefined && quantity > 0) {
      params = params.set('Quantity', quantity.toString());
    }

    if (isActive !== undefined) {
      params = params.set('IsActive', isActive.toString());
    }

    return this.http.get<ApiResponseDto>(url, { params: params }).pipe(
      map(mapApiResponse<ListBillingPlansDto>),
      map((res) => res.billingPlans),
    );
  }

  private mapBillingPlanFromApi(
    apiBillingPlan: BillingPlanDetailsApiDto['billingPlan'],
  ): BillingPlan {
    return {
      id: apiBillingPlan.id,
      group: apiBillingPlan.group,
      dailyBilling: apiBillingPlan.dailyBilling,
      controlledBilling: apiBillingPlan.controlledBilling,
      freeBilling: apiBillingPlan.freeBilling,
      isActive: apiBillingPlan.isActive,
    };
  }
}
