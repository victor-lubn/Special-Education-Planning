export enum SortDirection {
  Ascending,
  Descending
}

export class SortDescriptor {

  constructor(
    public member: string,
    public direction: SortDirection
  ) {}

}
