import { EmployeeDataPayload } from './employee.models';

export type ApiResponseDto =
  | { success: true; data: ApiResponseDataPayload }
  | { success: false; errors: string[] };

export type ApiResponseDataPayload = EmployeeDataPayload | IdApiResponse;

export interface IdApiResponse {
  id: string;
}
