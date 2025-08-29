import { FilterOperator } from "src/app/core/services/url-parser/filter-descriptor.model";

export interface ResolveFilterDescriptorsMapper {
  operator: FilterOperator,
  path: string,
  member: string,
  resolver?: (value: any) => string
};
