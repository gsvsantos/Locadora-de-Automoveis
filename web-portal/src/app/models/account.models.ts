export interface ClientProfile {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  document: string;
  address?: Address;
}

export interface ClientProfileApiDto {
  client: ClientProfile;
}

export interface UpdateLanguageDto {
  language: string;
}

export interface Address {
  state: string;
  city: string;
  neighborhood: string;
  street: string;
  number: number;
}
