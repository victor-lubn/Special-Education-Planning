import { SortDescriptor } from './sort-descriptor.model';
import { FilterDescriptor } from './filter-descriptor.model';

export class PageDescriptor {

  private filters: Array<FilterDescriptor>;
  private sorts: Array<SortDescriptor>;
  private skip: number;
  private take: number;
  private envelope: boolean;

  constructor() {
    this.filters = [];
    this.sorts = [];
    this.skip = null;
    this.take = null;
    this.envelope = true;
  }

  public initFilters(filters: FilterDescriptor[]) {
    this.filters = [...filters];
  }

  /**
   * Temporary
   */
  public addFilter(filter: FilterDescriptor): void {
    this.filters.push(filter);
  }

  public addFilters(filters: FilterDescriptor[]): void {
    this.filters = [ ...this.filters, ...filters ];
  }

  public addOrUpdateFilter(filter: FilterDescriptor): void {
    const foundFilter = this.filters.find(currentFilter => currentFilter.member === filter.member);
    if (foundFilter) {
      this.filters.splice(this.filters.indexOf(foundFilter), 1, filter);
    } else {
      this.filters.push(filter);
    }
  }

  public addOrUpdateFilters(filters: FilterDescriptor[]): void {
    filters.forEach(filter => {
      this.addOrUpdateFilter(filter);
    });
  }

  public deleteFilter(filterName: string): void {
    this.filters = this.filters.filter(currentFilter => currentFilter.member !== filterName);
  }

  public deleteAllFilters(): void {
    this.filters = [];
  }

  public deleteFilters(filterNames: string[]): void {
    this.filters = this.filters.filter(({ member }) => !filterNames.includes(member));
  }

  public getFilters(): Array<FilterDescriptor> {
    return this.filters;
  }

  public getFiltersNames(): Array<string> {
    return this.filters.map((filter) => {
      return filter.member;
    });
  }

  public initSorts(sorts: SortDescriptor[]) {
    this.sorts = [...sorts];
  }

  public addOrUpdateSort(sort: SortDescriptor): void {
    const foundSort = this.sorts.find(currentSort => currentSort.member === sort.member);
    if (foundSort) {
      this.sorts.splice(this.sorts.indexOf(foundSort), 1, sort);
    } else {
      this.sorts.push(sort);
    }
  }

  public deleteSort(sortName: string): void {
    this.sorts = this.sorts.filter(currentSort => currentSort.member !== sortName);
  }

  public deleteAllSorts(): void {
    this.sorts = [];
  }

  public getSorts(): Array<SortDescriptor> {
    return this.sorts;
  }

  public getSortsNames(): Array<string> {
    return this.sorts.map((sort) => {
      return sort.member;
    });
  }

  public setPagination(pageNumber: number, pageSize: number): void {
    this.take = pageSize;
    this.skip = pageNumber > 0 ? pageSize * pageNumber : 0;
  }

  public getTake(): number {
    return this.take;
  }

  public getSkip(): number {
    return this.skip;
  }

  public setEnvelope(value: boolean): void {
    this.envelope = value;
  }

  public getEnvelope(): boolean {
    return this.envelope;
  }

}
