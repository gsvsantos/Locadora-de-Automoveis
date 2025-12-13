export interface RegisterAuthDto {
  userName: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
}

export interface LoginAuthDto {
  userName: string;
  password: string;
}

export interface AuthApiResponse {
  key: string;
  expiration: Date;
  user: AuthenticatedUserModel;
}

export interface AuthenticatedUserModel {
  id: string;
  fullName: string;
  userName: string;
  email: string;
  phoneNumber: string;
  roles: string[];
}
