import { AuthApiResponse } from '../models/auth.models';

export function getHeaderAuthorizationOptions(response?: AuthApiResponse): object {
  if (!response) throw new Error('The access token was not provided');

  return {
    headers: {
      Authorization: 'Bearer ' + response.key,
    },
  };
}
