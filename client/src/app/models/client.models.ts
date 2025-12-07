export interface ClientDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  type: string;
  document: string;
}

export interface CreateClientDto extends ClientDto {
  state: string;
  city: string;
  neighborhood: string;
  street: string;
  number: number;
}

export interface Client extends ClientDto {
  id: string;
  address: Address;
  licenseNumber: string;
  isActive: boolean;
}

export interface ListClientsDto {
  quantity: number;
  clients: Client[];
}

export interface ClientDetailsApiDto {
  client: Client;
}

export type ClientDataPayload = ListClientsDto;

export interface Address {
  state: string;
  city: string;
  neighborhood: string;
  street: string;
  number: number;
}
