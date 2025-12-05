import { HttpErrorResponse } from '@angular/common/http';
import { ApiResponseDto } from '../models/api.models';
import { Observable, throwError } from 'rxjs';

export function mapApiResponse<T>(res: ApiResponseDto): T {
  if (!res.success) {
    const messages =
      Array.isArray(res.errors) && res.errors.length > 0 ? res.errors : ['Unexpected API error.'];

    throw new Error(messages.join('\n'));
  }

  return res.data as T;
}

export function mapApiErroResponse(res: HttpErrorResponse): Observable<never> {
  const payload: unknown = res.error;

  if (Array.isArray(payload)) {
    const messages = payload.filter(
      (item: unknown): item is string => typeof item === 'string' && item.trim().length > 0,
    );

    if (messages.length > 0) {
      return throwError(() => messages.join('\n'));
    }
  }

  if (payload && typeof payload === 'object' && 'success' in payload) {
    const apiError = payload as Partial<ApiResponseDto<unknown>>;

    if (apiError.success === false && Array.isArray(apiError.errors)) {
      return throwError(() => apiError.errors?.join('\n'));
    }
  }

  return throwError(
    () => 'Unexpected error while calling the API. Check the console for more details.',
  );
}
