import { Component, OnInit, ViewEncapsulation } from '@angular/core';

import { SystemLog } from '../../../../shared/models/system-log';
import { ListComponent } from '../../../../shared/base-classes/list-component';
import { ComponentReloadData } from '../../../../shared/base-classes/reload-data-view';
import { ApiService } from '../../../../core/api/api.service';
import { AppEntitiesEnum } from '../../../../shared/models/app-enums';
import { SortDescriptor, SortDirection } from '../../../../core/services/url-parser/sort-descriptor.model';
import { TableColumnConfig, TableRecords } from '../../../../shared/components/organisms/table/table.types';
import { PageEvent } from '@angular/material/paginator';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { SelectOptionInterface } from '../../../../shared/components/atoms/select/select.component';
import { TranslateService } from '@ngx-translate/core';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { SortingFilteringItemsService } from '../../../../core/services/sorting-filtering-items/sortingFilteringItems.service';
import { debounceTime } from 'rxjs/operators';
import { FilterOperator } from '../../../../core/services/url-parser/filter-descriptor.model';

@Component({
  selector: 'tdp-system-logs-list',
  templateUrl: './system-logs-list.component.html',
  styleUrls: ['./system-logs-list.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SystemLogsListComponent extends ListComponent<SystemLog> implements OnInit, ComponentReloadData {

  public options: SelectOptionInterface[];
  public sortDescending: SortDescriptor;
  public systemLogs: TableRecords<SystemLog> = { data: []};
  public systemLogsConfiguration: TableColumnConfig[] = [];
  public pageSize: number = 7;
  public systemLogsOptions: SelectOptionInterface[] = [];
  public form: UntypedFormGroup;
  public delayTime: number = 400;
  //Translation strings
  private levelHeader: string = '';
  private messageHeader: string = '';
  private messageTemplateHeader: string = '';
  private exceptionHeader: string = '';
  private propertiesHeader: string = '';
  private dateHeader: string = '';
  private logDetailErrorTitle: string = '';
  private logDetailErrorMessage: string = '';

  constructor(
    private api: ApiService,
    private dialogs: DialogsService,
    private translate: TranslateService,
    private sortingFiltering: SortingFilteringItemsService
  ) {
    super();
    this.sortDescending = new SortDescriptor ('TimeStamp', SortDirection.Descending);
  }

  ngOnInit() {
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.initializeTranslations();
    this.initializeColumnConfiguration();
    this.getSortingFilteringItems();
    this.createForm();
    this.recoverViewData();
    const filterSubscription = this.form.valueChanges.pipe(debounceTime(this.delayTime)).subscribe(response => {
      if (response.filterBy) {
        this.pageDescriptor.deleteAllFilters();
        this.pageDescriptor.addOrUpdateFilters([
          {
            member: response.filterBy,
            value: response.search,
            operator: FilterOperator.Contains
          }
        ])
        this.reloadDataView();
      }
    })
    this.entitySubscriptions.push(filterSubscription);
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

  public openPreviewDialog(event: MouseEvent, record: SystemLog) {
    event.stopPropagation();
    event.preventDefault();
    if (record.messageTemplate != null || record.messageTemplate !== '') {
      this.dialogs.systemLogDetail(record);
    } else {
      this.dialogs.information(this.logDetailErrorTitle, this.logDetailErrorMessage);
    }
  }

  protected recoverViewData(): void {
    const subscription = this.api.systemLogs.getLogs(this.pageDescriptor)
      .subscribe((response) => {
        this.systemLogs = response;
      });
    this.entitySubscriptions.push(subscription);
  }

  private getSortingFilteringItems(): void {
    this.sortingFiltering.getOptions(AppEntitiesEnum.systemLog).then(options$ => {
      const subscription = options$.subscribe(options => {
          this.options = options;
        }
      );
      this.entitySubscriptions.push(subscription);
    });
  }

  private createForm() {
    this.form = new UntypedFormGroup({
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl('')
    })
  }

  private initializeColumnConfiguration() {
    this.systemLogsConfiguration = [
      { columnDef: 'level', header: this.levelHeader, sortField: 'Level'},
      { columnDef: 'message', header: this.messageHeader, sortField: 'Message', tooltipAtLength: 32 },
      { columnDef: 'messageTemplate', header: this.messageTemplateHeader, sortField: 'MessageTemplate', tooltipAtLength: 32 },
      { columnDef: 'exception', header: this.exceptionHeader, sortField: 'Exception' },
      { columnDef: 'properties', header: this.propertiesHeader, sortField: 'Properties', tooltipAtLength: 17 },
      { columnDef: 'timeStamp', header: this.dateHeader, sortField: 'TimeStamp', isDate: true },
    ]
  }

  private initializeTranslations() {
    const translationSubscription = this.translate.get([
      'systemLog.level',
      'systemLog.message',
      'systemLog.messageTemplate',
      'systemLog.exception',
      'systemLog.properties',
      'systemLog.dateTime',
      'dialog.logDetailError.title',
      'dialog.logDetailError.message'
    ]).subscribe((translations) => {
      this.levelHeader = translations['systemLog.level'];
      this.messageHeader = translations['systemLog.message'];
      this.messageTemplateHeader = translations['systemLog.messageTemplate'];
      this.exceptionHeader = translations['systemLog.exception'];
      this.propertiesHeader = translations['systemLog.properties'];
      this.dateHeader = translations['systemLog.dateTime'];
      this.logDetailErrorTitle = translations['dialog.logDetailError.title'];
      this.logDetailErrorMessage = translations['dialog.logDetailError.message'];
    });
    this.entitySubscriptions.push(translationSubscription);
  }
}
