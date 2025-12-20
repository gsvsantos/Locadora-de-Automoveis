export interface PartnerDto {
  fullName: string;
}

export interface Partner extends PartnerDto {
  id: string;
  isActive: boolean;
}
