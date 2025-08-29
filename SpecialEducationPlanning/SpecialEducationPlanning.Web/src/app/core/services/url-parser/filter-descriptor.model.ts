export enum FilterOperator {
  IsLessThan,
  IsLessThanOrEqualTo,
  IsEqualTo,
  IsNotEqualTo,
  IsGreaterThan,
  IsGreaterThanOrEqualTo,
  StartsWith,
  EndsWith,
  Contains,
  DoesNotContain,
  IsContainedIn,
  IsNotContainedIn
}

export class FilterDescriptor {

  public member: string;
  public operator: FilterOperator;
  public value: string;

  constructor(
    member: string,
    operator: FilterOperator,
    value: string | Array<string | number> | Date
  ) {
    this.member = member;
    this.operator = operator;
    if (value instanceof Date) {
      this.value = value.toISOString();
    } else {
      this.value = (value || '').toString();
    }
  }

}
