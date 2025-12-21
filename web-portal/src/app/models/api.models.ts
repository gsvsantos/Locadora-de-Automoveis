export type ApiResponseDto =
  | { success: true; data: ApiResponseDataPayload }
  | { success: false; errors: string[] };

export type ApiResponseDataPayload = IdApiResponse;

export interface IdApiResponse {
  id: string;
}
