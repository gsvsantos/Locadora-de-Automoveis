export interface GroupDto {
  name: string;
}

export interface Group extends GroupDto {
  id: string;
  isActive: boolean;
}

export interface ListGroupsDto {
  quantity: number;
  groups: Group[];
}

export interface GroupDetailsApiDto {
  group: {
    id: string;
    name: string;
    isActive: boolean;
  };
}

export type GroupDataPayload = ListGroupsDto;
