export interface ClientProfile {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  document: string;
}

export interface ClientProfileApiDto {
  client: ClientProfile;
}
