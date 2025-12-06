import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, map, catchError } from 'rxjs';
import { mapApiResponse, mapApiErroResponse } from '../utils/map-api-response';
import { ApiResponseDto } from '../models/api.models';

export const apiResponseInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  return next(req).pipe(
    map((httpEvent: HttpEvent<unknown>) => {
      if (httpEvent instanceof HttpResponse) {
        const body = httpEvent.body;

        if (body && typeof body === 'object' && 'success' in body) {
          const apiResponseDto = body as ApiResponseDto<unknown>;
          const data = mapApiResponse<unknown>(apiResponseDto);

          return httpEvent.clone({ body: data });
        }
      }

      return httpEvent;
    }),
    catchError((httpErrorResponse: HttpErrorResponse) => mapApiErroResponse(httpErrorResponse)),
  );
};
