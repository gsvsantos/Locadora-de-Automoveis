export interface ExtraDto {
  name: string;
  price: number;
  isDaily: boolean;
  type: string;
}

export interface Extra extends ExtraDto {
  id: string;
  isActive: boolean;
}

export interface ListExtrasDto {
  quantity: number;
  extras: Extra[];
}

export interface ExtraDetailsApiDto {
  extra: Extra;
}

export type ExtraDataPayload = ListExtrasDto;

export interface Address {
  state: string;
  city: string;
  neighborhood: string;
  street: string;
  number: number;
}
