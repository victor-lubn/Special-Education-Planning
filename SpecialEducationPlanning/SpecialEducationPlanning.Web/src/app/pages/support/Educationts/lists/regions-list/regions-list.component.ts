import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime, filter } from 'rxjs/operators';
import { NotificationsService } from 'angular2-notifications';
import { ApiService } from '../../../../../core/api/api.service';
import { CommunicationService } from '../../../../../core/services/communication/communication.service';
import { DialogsService } from '../../../../../core/services/dialogs/dialogs.service';
import { EnvelopeResponse } from '../../../../../core/services/url-parser/envelope-response.interface';
import { FilterDescriptor, FilterOperator } from '../../../../../core/services/url-parser/filter-descriptor.model';
import { SortDescriptor } from '../../../../../core/services/url-parser/sort-descriptor.model';
import { ListComponent } from '../../../../../shared/base-classes/list-component';
import { ComponentReloadData } from '../../../../../shared/base-classes/reload-data-view';
import { SelectOptionInterface } from '../../../../../shared/components/atoms/select/select.component';
import { AppEntitiesEnum } from '../../../../../shared/models/app-enums';
import { Region } from '../../../../../shared/models/region';
import { SortingFilteringItemsService } from '../../../../../core/services/sorting-filtering-items/sortingFilteringItems.service';
import { PageDescriptor } from 'src/app/core/services/url-parser/page-descriptor.model';
import { TableColumnConfig } from 'src/app/shared/components/organisms/table/table.types';
@Component({
  selector: 'tdp-regions-list',
  templateUrl: './regions-list.component.html',
  styleUrls: ['./regions-list.component.scss']
})
export class RegionsListComponent extends ListComponent<Region> implements OnInit, ComponentReloadData {
  form: UntypedFormGroup;
  options: SelectOptionInterface[]
  regions: EnvelopeResponse<Region>
  regionsTitleString: string;
  regionString: string;
  areaString: string;
  columnsConfiguration: TableColumnConfig[];
  public ms: number = 400;

  public countryId: number;
  public coutryKeyName: string;
  public pageSize = 7;

  public countryFilter: FilterDescriptor;

  constructor(
    private translate: TranslateService,
    private api: ApiService,
    private communication: CommunicationService,
    private activatedRoute: ActivatedRoute,
    private dialogs: DialogsService,
    private notifications: NotificationsService,
    private communications: CommunicationService,
    private sortingFiltering: SortingFilteringItemsService
  ) {
    super();
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
  }

  ngOnInit() {
    const routerSubscription = this.activatedRoute.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.countryId = +queryParams.get('id');
      this.coutryKeyName = queryParams.get('keyName');
      this.initializeTranslationStrings()
      this.initializeColumnConfiguration()
    });
    this.entitySubscriptions.push(routerSubscription);

    this.countryFilter = new FilterDescriptor('CountryId', FilterOperator.IsEqualTo, [this.countryId]);
    this.sortingFiltering.getOptions(AppEntitiesEnum.region).then(options$ => {
      const subscription = options$.subscribe(options => {
        this.options = options;
      }
      );
      this.entitySubscriptions.push(subscription);
    });
    this.recoverViewData();
    const subscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(subscription);
    this.createForm();
    const suscription = this.form.valueChanges.pipe(
      debounceTime(this.ms),
      filter(response => response.filterBy))
      .subscribe(response => {
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
      });
    this.entitySubscriptions.push(suscription);
  }

  initializeColumnConfiguration(): void {
    this.columnsConfiguration = [
      { columnDef: 'keyName', header: this.regionString, sortField: 'keyName', tooltipAtLength: 57 },
      { columnDef: 'areasCount', header: this.areaString, sortField: 'areasCount', tooltipAtLength: 50 },
    ];
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'regionsPage.title',
      'regionsPage.region',
      'regionsPage.areas'
    ]).subscribe(translations => {
      this.regionsTitleString = `${translations['regionsPage.title']} (${this.coutryKeyName})`;
      this.regionString = translations['regionsPage.region'];
      this.areaString = translations['regionsPage.areas'];
    })
    this.entitySubscriptions.push(translationSubscription);
  }

  reloadDataView(): void {
    this.recoverViewData();
  }

  public openEditRegion(event, region: Region): void {
    event.stopPropagation()
    this.dialogs.createEditRegion(this.countryId, region);
  }

  public openCreateRegion(): void {
    this.dialogs.createEditRegion(this.countryId);
  }

  protected recoverViewData(): void {
    this.pageDescriptor.addOrUpdateFilter(this.countryFilter);
    const subscription = this.api.regions.getRegions(this.pageDescriptor)
      .subscribe((response) => {
        this.regions = response;
      });
    this.entitySubscriptions.push(subscription);
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

  openRegionDetails(region: Region) {
    this.navigateTo('/support/Aieps/areas', { id: region.id, keyName: region.keyName })
  }

  public removeRegion(event, region: Region): void {
    event.stopPropagation();
    this.dialogs.confirmation('dialog.removeRegion', 'dialog.removeRegionMessage')
      .then((confirmation) => {
        if (confirmation) {
          const subscription = this.api.regions.deleteRegion(region.id)
            .subscribe((success) => {
              this.notifications.success(this.translations['notification.regionDeletedSuccess']);
              this.communications.notifyReloadViewData();
            }, (error) => {
              this.notifications.error(this.translations['notification.regionDeletedError']);
            });
          this.entitySubscriptions.push(subscription);
        }
      });
  }
}

