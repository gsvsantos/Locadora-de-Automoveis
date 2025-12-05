export type ApiResponseDto<T = unknown> =
  | { success: true; data: T }
  | { success: false; errors: string[] };

export interface IdApiResponse {
  id: string;
}
