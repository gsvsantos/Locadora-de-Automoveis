import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { IdApiResponse, ApiResponseDto } from '../models/api.models';
import {
  ConfigurationDto,
  Configuration,
  ConfigutarionDetailsDto,
  CombustivelApiDto,
  Prices,
} from '../models/configuration.models';
import { mapApiResponse } from '../utils/map-api-response';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  private readonly apiUrl: string = environment.apiUrl + '/configuration';
  private readonly http: HttpClient = inject(HttpClient);

  public configure(registerModel: ConfigurationDto): Observable<IdApiResponse> {
    const url = `${this.apiUrl}/configure`;

    return this.http.post<IdApiResponse>(url, registerModel);
  }
  public details(): Observable<Configuration> {
    const url: string = `${this.apiUrl}/details`;

    return this.http.get<ApiResponseDto>(url).pipe(
      map(mapApiResponse<ConfigutarionDetailsDto>),
      map((apiDto: ConfigutarionDetailsDto) => this.mapDriverFromApi(apiDto.configuration)),
    );
  }

  private mapDriverFromApi(apiConfig: ConfigutarionDetailsDto['configuration']): Configuration {
    return {
      id: apiConfig.id,
      gasolinePrice: apiConfig.gasolinePrice,
      gasPrice: apiConfig.gasPrice,
      dieselPrice: apiConfig.dieselPrice,
      alcoholPrice: apiConfig.alcoholPrice,
    };
  }
}
