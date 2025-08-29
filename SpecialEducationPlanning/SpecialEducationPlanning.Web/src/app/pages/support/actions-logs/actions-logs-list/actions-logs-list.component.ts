import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime } from 'rxjs/operators';
import { SortingFilteringItemsService } from 'src/app/core/services/sorting-filtering-items/sortingFilteringItems.service';
import { SelectOptionInterface } from 'src/app/shared/components/atoms/select/select.component';
import { ApiService } from '../../../../core/api/api.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { DownloadFileService } from '../../../../core/services/download-file/download-file.service';
import { FilterDescriptor, FilterOperator } from '../../../../core/services/url-parser/filter-descriptor.model';
import { PageDescriptor } from '../../../../core/services/url-parser/page-descriptor.model';
import { SortDescriptor, SortDirection } from '../../../../core/services/url-parser/sort-descriptor.model';
import { ListComponent } from '../../../../shared/base-classes/list-component';
import { ComponentReloadData } from '../../../../shared/base-classes/reload-data-view';
import { TableService } from '../../../../shared/components/organisms/table/table.service';
import { TableColumnConfig, TableRecords } from '../../../../shared/components/organisms/table/table.types';
import { ActionLogs } from '../../../../shared/models/action-logs';
import { ActionTypeEnum, AppEntitiesEnum } from '../../../../shared/models/app-enums';
import { DateRange } from '../../../../shared/models/date-range';

@Component({
  selector: 'tdp-actions-logs-list',
  templateUrl: 'actions-logs-list.component.html',
  styleUrls: ['actions-logs-list.component.scss'],
  providers: [TableService]
})
export class ActionsLogsListComponent extends ListComponent<ActionLogs> implements OnInit, ComponentReloadData {

  public options: SelectOptionInterface[];
  public sortDescending: SortDescriptor;
  public dateRange: DateRange;
  public actionsColumnsConfiguration: TableColumnConfig[] = [];
  public actionLogs: TableRecords<ActionLogs> = { data: [] };
  public pageSize: number = 7;
  public optionsTranslated: Record<ActionTypeEnum, string>;
  public maximumDate: Date = new Date();

  //Translation strings
  public actionHeader: string = '';
  public userHeader: string = '';
  public belongsToHeader: string = '';
  public valueHeader: string = '';
  public dateHeader: string = '';
  form: UntypedFormGroup;
  #ms: number = 400;

  constructor(
    private api: ApiService,
    private sortingFiltering: SortingFilteringItemsService,
    private downloadService: DownloadFileService,
    private dialogs: DialogsService,
    private translate: TranslateService
  ) {
    super();
    this.actionLogs = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.sortDescending = new SortDescriptor('Date', SortDirection.Descending);
  }

  ngOnInit() {
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.getFilteringOptions();
    this.initializaTranslations();
    this.recoverViewData();
    this.initializeColumnConfiguration();
    this.createForm();
    const subscription = this.form.valueChanges.pipe(debounceTime(this.#ms)).subscribe(response => {

      if (response.filterBy) {
        this.pageDescriptor.deleteAllFilters();
        this.pageDescriptor.addOrUpdateFilters([
          {
            member: response.filterBy,
            value: response.search,
            operator: FilterOperator.Contains
          }
        ]);
      }
      if (response.range.start && response.range.end) {
        this.dateRange = {
          startDate: response.range.start,
          endDate: response.range.end
        }}
      this.setPageDescriptorDateRange(response.range.start, response.range.end)
      this.reloadDataView();
    })
    this.entitySubscriptions.push(subscription);
  }

  reloadDataView(): void {
    this.recoverViewData();
  }

  public pageChanged(event: PageEvent) {
    this.pageDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.reloadDataView();
  }

  public sortChanged(event: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(event);
    this.reloadDataView();
  }

  protected recoverViewData(): void {
    const actionLogsSubscription = this.api.actionLogs.getActionLogs(this.pageDescriptor)
      .subscribe((response) => {
        this.actionLogs = response;
      });
    this.entitySubscriptions.push(actionLogsSubscription);
  }

  public setPageDescriptorDateRange(dateRangeStart: Date, dateRangeEnd: Date) {

    const filters = this.pageDescriptor.getFilters();
    const noDateFilter = filters.find(x => x.member !== 'Date');

    this.pageDescriptor.deleteAllFilters();

    if (noDateFilter) { this.pageDescriptor.addOrUpdateFilter(noDateFilter); }

    if (dateRangeStart) {
      this.pageDescriptor.addFilter(
        new FilterDescriptor('Date', FilterOperator.IsGreaterThanOrEqualTo, dateRangeStart)
      );
    }

    if (dateRangeEnd) {
      this.pageDescriptor.addFilter(
        new FilterDescriptor('Date', FilterOperator.IsLessThanOrEqualTo, dateRangeEnd)
      );
    }
  }

  public exportToCsv(): void {
    if (this.dateRange && this.dateRange.startDate && this.dateRange.endDate) {
      const actionLogsSubscription = this.api.actionLogs.getActionLogsCSV(this.dateRange.startDate, this.dateRange.endDate)
        .subscribe((response) => {
          if (response) {
            this.downloadService.downloadFile(
              new Blob([response], { type: 'text/csv' }),
              'action_logs' + new Date().toLocaleDateString() + '.csv'
            );
          }
        });
      this.entitySubscriptions.push(actionLogsSubscription);
    } else {
      this.dialogs.unableSupportsLog();
    }
  }

  private getFilteringOptions(){
    this.sortingFiltering.getOptions(AppEntitiesEnum.actionLogs).then(options$ => {
      const subscription = options$.subscribe(options => {
          this.options = options;
        }
      );
      this.entitySubscriptions.push(subscription);
    });
  }

  private initializeColumnConfiguration() {
    this.actionsColumnsConfiguration = [
      {
        columnDef: 'actionType', header: this.actionHeader, custom: false,
        callback: (record: any) => {
          return this.optionsTranslated[record.actionType]
        },
        sortField: 'ActionType'
      },
      { columnDef: 'user', header: this.userHeader, sortField: 'User', tooltipAtLength: 29 },
      { columnDef: 'entityName', header: this.belongsToHeader, sortField: 'EntityName', tooltipAtLength: 26 },
      { columnDef: 'entityValue', header: this.valueHeader, sortField: 'EntityValue', tooltipAtLength: 25 },
      { columnDef: 'date', header: this.dateHeader, sortField: 'Date', isDate: true }
    ]
  }

  private initializaTranslations() {
    const translationStrings = this.translate.get([
      'actionLogs.action',
      'actionLogs.user',
      'actionLogs.belongsTo',
      'actionLogs.value',
      'actionLogs.date',
      'actionLogs.messages.entityCreateMessage',
      'actionLogs.messages.entityUpdateMessage',
      'actionLogs.messages.entityDeleteMessage',
      'actionLogs.messages.planPublishMessage',
      'actionLogs.messages.fileCreateMessage',
      'actionLogs.messages.fileUpdateMessage'
    ]).subscribe((translations) => {
      this.actionHeader = translations['actionLogs.action'];
      this.userHeader = translations['actionLogs.user'];
      this.belongsToHeader = translations['actionLogs.belongsTo'];
      this.valueHeader = translations['actionLogs.value'];
      this.dateHeader = translations['actionLogs.date'];
      this.optionsTranslated = {
        [ActionTypeEnum.Create]: translations['actionLogs.messages.entityCreateMessage'],
        [ActionTypeEnum.Update]: translations['actionLogs.messages.entityUpdateMessage'],
        [ActionTypeEnum.Delete]: translations['actionLogs.messages.entityDeleteMessage'],
        [ActionTypeEnum.Publish]: translations['actionLogs.messages.planPublishMessage'],
        [ActionTypeEnum.FileCreate]: translations['actionLogs.messages.fileCreateMessage'],
        [ActionTypeEnum.FileUpdate]: translations['actionLogs.messages.fileUpdateMessage']
      }
    })
    this.entitySubscriptions.push(translationStrings);
  }

  private createForm() {
    this.form = new UntypedFormGroup({
      range: new UntypedFormGroup({
        start: new UntypedFormControl(null),
        end: new UntypedFormControl(null)
      }),
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl('')
    })
  }
}
