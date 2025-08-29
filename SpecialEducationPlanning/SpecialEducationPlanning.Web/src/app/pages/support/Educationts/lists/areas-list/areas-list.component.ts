import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { debounceTime, filter } from 'rxjs/operators';
import { TableColumnConfig } from 'src/app/shared/components/organisms/table/table.types';
import { ApiService } from '../../../../../core/api/api.service';
import { CommunicationService } from '../../../../../core/services/communication/communication.service';
import { DialogsService } from '../../../../../core/services/dialogs/dialogs.service';
import { SortingFilteringItemsService } from '../../../../../core/services/sorting-filtering-items/sortingFilteringItems.service';
import { EnvelopeResponse } from '../../../../../core/services/url-parser/envelope-response.interface';
import { FilterDescriptor, FilterOperator } from '../../../../../core/services/url-parser/filter-descriptor.model';
import { SortDescriptor } from '../../../../../core/services/url-parser/sort-descriptor.model';
import { ListComponent } from '../../../../../shared/base-classes/list-component';
import { ComponentReloadData } from '../../../../../shared/base-classes/reload-data-view';
import { SelectOptionInterface } from '../../../../../shared/components/atoms/select/select.component';
import { AppEntitiesEnum } from '../../../../../shared/models/app-enums';
import { Area } from '../../../../../shared/models/area';


@Component({
  selector: 'tdp-areas-list',
  templateUrl: './areas-list.component.html',
  styleUrls: ['./areas-list.component.scss']
})
export class AreasListComponent extends ListComponent<Area> implements OnInit, ComponentReloadData {
  form: UntypedFormGroup;
  options: SelectOptionInterface[]
  areas: EnvelopeResponse<Area>
  regionTitleString: string;
  areaString: string;
  AiepsString: string;
  ms: number = 400;
  columnsConfiguration: TableColumnConfig[];

  public regionId: number;
  public regionKeyName: string;
  public regionFilter: FilterDescriptor;
  public pageSize = 7;

  constructor(
    private translate: TranslateService,
    private api: ApiService,
    private activatedRoute: ActivatedRoute,
    private communication: CommunicationService,
    private dialogs: DialogsService,
    private notifications: NotificationsService,
    private sortingFiltering: SortingFilteringItemsService
  ) {
    super();
  }


  ngOnInit() {
    const routerSubscription = this.activatedRoute.queryParamMap
      .subscribe((queryParams: ParamMap) => {
        this.regionId = +queryParams.get('id');
        this.regionKeyName = queryParams.get('keyName');
        this.initializeTranslationStrings()
        this.initializeColumnConfiguration()
      });
    this.entitySubscriptions.push(routerSubscription);

    this.regionFilter = new FilterDescriptor('RegionId', FilterOperator.IsEqualTo, [this.regionId]);
    this.createForm();
    this.recoverViewData();
    this.sortingFiltering.getOptions(AppEntitiesEnum.area).then(options$ => {
      const subscription = options$.subscribe(options => {
        this.options = options;
      });
      this.entitySubscriptions.push(subscription);
    });
    const filterSubscription = this.form.valueChanges.pipe(
      debounceTime(this.ms),
      filter(response => response.filterBy)
      ).subscribe(response => {
      this.pageDescriptor.deleteAllFilters();
      if (response.search) {
        this.pageDescriptor.addOrUpdateFilter(
          {
            member: response.filterBy,
            value: response.search,
            operator: FilterOperator.Contains
          }
        );
      }
      this.reloadDataView();
    })
    this.entitySubscriptions.push(filterSubscription);
    const subscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(subscription);
  }

  initializeColumnConfiguration(): void {
    this.columnsConfiguration = [
      { columnDef: 'keyName', header: this.areaString, sortField: 'keyName', tooltipAtLength: 57 },
      { columnDef: 'AiepCount', header: this.AiepsString, sortField: 'AiepCount', tooltipAtLength: 50 },
    ];
  }


  reloadDataView(): void {
    this.recoverViewData();
  }

  public openCreateArea(): void {
    this.dialogs.createEditArea(this.regionId);
  }

  public openEditArea(event, area: Area): void {
    event.stopPropagation()
    this.dialogs.createEditArea(this.regionId, area);
  }

  protected recoverViewData(): void {
    this.pageDescriptor.addOrUpdateFilter(this.regionFilter);
    this.pageDescriptor.setPagination(0, this.pageSize);
    const subscription = this.api.areas.getAreas(this.pageDescriptor)
      .subscribe((response) => {
        this.areas = response;
      });
    this.entitySubscriptions.push(subscription);
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'areasPage.title',
      "areasPage.area",
      "areasPage.Aieps"
    ]).subscribe(translations => {
      this.regionTitleString = `${translations['areasPage.title']} (${this.regionKeyName})`;
      this.areaString = translations['areasPage.area']
      this.AiepsString = translations['areasPage.Aieps']
    })
    this.entitySubscriptions.push(translationSubscription);
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

  private createForm() {
    this.form = new UntypedFormGroup({
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl(null)
    })
  }

  removeArea(event, area: Area) {
    event.stopPropagation();
    this.dialogs.confirmation('dialog.removeArea', 'dialog.removeAreaMessage')
      .then((confirmation) => {
        if (confirmation) {
          const subscription = this.api.areas.deleteArea(area.id)
            .subscribe((success) => {
              this.notifications.success(this.translations['notification.areaDeletedSuccess']);
              this.communication.notifyReloadViewData();
            }, (error) => {
              this.notifications.error(this.translations['notification.areaDeletedError']);
            });
          this.entitySubscriptions.push(subscription);
        }
      });
  }
}

