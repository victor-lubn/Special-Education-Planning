import { BaseComponent } from './base-component';
import { PageDescriptor } from '../../core/services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../core/services/url-parser/envelope-response.interface';
import { SortingFiltering } from '../models/sorting-filtering';
import { PaginationChangeEvent } from '../models/pagination-change-event';
import { SortDescriptor } from '../../core/services/url-parser/sort-descriptor.model';
import { FilterDescriptor, FilterOperator } from '../../core/services/url-parser/filter-descriptor.model';

/**
 * List Component class
 *
 * Application components should extend this class to reuse Entities List base functionality
 */
export abstract class ListComponent<T> extends BaseComponent {

  public entityEnvelopeResponse: EnvelopeResponse<T>;
  public defaultSelectedPage: number;
  public defaultPageSize: number;
  public defaultMaxPagesShowing: number;
  public defaultPageSizeOptions: number[];

  public sortingFilteringItems: SortingFiltering[];
  public filteringOperator: FilterOperator;

  protected pageDescriptor: PageDescriptor;

  protected abstract recoverViewData(): void;

  constructor(
    defaultSelectedPage: number = 1,
    defaultPageSize: number = 6,
    defaultMaxPagesShowing: number = 4,
    defaultPageSizeOptions: number[] = [4, 6, 8]
  ) {
    super();
    this.defaultSelectedPage = defaultSelectedPage;
    this.defaultPageSize = defaultPageSize;
    this.defaultMaxPagesShowing = defaultMaxPagesShowing;
    this.defaultPageSizeOptions = defaultPageSizeOptions;
    this.sortingFilteringItems = [];
    this.resetPageDescriptor();
  }

  public handlePaginationChanges(event: PaginationChangeEvent): void {
    this.pageDescriptor.setPagination(event.pageNumber, event.pageSize);
    this.recoverViewData();
  }

  public setPageDescriptorSort(sortDescriptor: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    if (
      sortDescriptor &&
      sortDescriptor.member &&
      sortDescriptor.direction >= 0
    ) {
      this.pageDescriptor.addOrUpdateSort(sortDescriptor);
    }
    this.recoverViewData();
  }

  public setPageDescriptorFilter(filterDescriptor: FilterDescriptor) {

    const filters = this.pageDescriptor.getFilters();
    const dateFilters = filters.filter(x => x.member === 'Date');

    this.pageDescriptor.deleteAllFilters();

    if (dateFilters) {
      dateFilters.forEach((filter) => {
        this.pageDescriptor.addFilter(filter);
      });
    }

    if (
      filterDescriptor &&
      filterDescriptor.value &&
      filterDescriptor.member &&
      filterDescriptor.operator >= 0
    ) {
      this.pageDescriptor.addOrUpdateFilter(filterDescriptor);
    }
    this.recoverViewData();
  }

  protected resetPageDescriptor(): void {
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(this.defaultSelectedPage, this.defaultPageSize);
  }

}
