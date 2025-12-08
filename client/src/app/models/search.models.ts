export interface GlobalSearchDto {
  term: string;
}

export interface GlobalSearch {
  id: string;
  title: string;
  description: string;
  type: string;
  route: string;
}

export interface ListGlobalSearchDto {
  items: GlobalSearch[];
}
