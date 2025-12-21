import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../../environments/environment';

export const googleAuthConfig: AuthConfig = {
  issuer: 'https://accounts.google.com',
  loginUrl: 'https://accounts.google.com/o/oauth2/v2/auth',
  tokenEndpoint: 'https://oauth2.googleapis.com/token',
  userinfoEndpoint: 'https://openidconnect.googleapis.com/v1/userinfo',
  revocationEndpoint: 'https://oauth2.googleapis.com/revoke',
  clientId: environment.client_id,
  redirectUri: window.location.origin,
  responseType: 'token id_token',
  scope: 'openid profile email',
  disablePKCE: true,
  strictDiscoveryDocumentValidation: false,
  customQueryParams: {
    access_type: 'online',
    prompt: 'consent',
  },
};
