import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../../../../core/api/api.service';
import { CommunicationService } from '../../../../../core/services/communication/communication.service';
import { ComponentReloadData } from '../../../../../shared/base-classes/reload-data-view';
import { Country } from '../../../../../shared/models/country.model';
import { AppEntitiesEnum } from '../../../../../shared/models/app-enums';
import { ListComponent } from '../../../../../shared/base-classes/list-component';
import { DialogsService } from '../../../../../core/services/dialogs/dialogs.service';
import { PageDescriptor } from 'src/app/core/services/url-parser/page-descriptor.model';
import { PageEvent } from '@angular/material/paginator';
import { SortDescriptor } from 'src/app/core/services/url-parser/sort-descriptor.model';
import { TableColumnConfig, TableRecords } from 'src/app/shared/components/organisms/table/table.types';
import { SelectOptionInterface } from 'src/app/shared/components/atoms/select/select.component';
import { SortingFilteringItemsService } from 'src/app/core/services/sorting-filtering-items/sortingFilteringItems.service';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { debounceTime, filter } from 'rxjs/operators';
import { FilterOperator } from 'src/app/core/services/url-parser/filter-descriptor.model';

@Component({
  selector: 'tdp-countries-list',
  templateUrl: 'countries-list.component.html',
  styleUrls: ['countries-list.component.scss']
})
export class CountriesListComponent extends ListComponent<Country> implements OnInit, ComponentReloadData {
  form: UntypedFormGroup;
  options: SelectOptionInterface[];
  public ms: number = 400;
  public columnsConfiguration: TableColumnConfig[] = [];
  readonly pageSize: number = 7;

  public countries: TableRecords<Country>;

  protected nameHeaderString: string = '';
  protected regionsHeaderString: string = '';
  protected countryDeletedSuccessString: string = '';
  protected countryDeletedErrorString: string = '';

  constructor(
    private api: ApiService,
    private dialogs: DialogsService,
    private communication: CommunicationService,
    private sortingFiltering: SortingFilteringItemsService,
    private translate: TranslateService,
    private notifications: NotificationsService
  ) {
    super();
    this.countries = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
  }

  ngOnInit() {
    this.sortingFiltering.getOptions(AppEntitiesEnum.country).then(options$ => {
      const subscription = options$.subscribe(options => {
        this.options = options;
      }
      );
      this.entitySubscriptions.push(subscription);
    });
    this.recoverViewData();
    const communicationSubscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(communicationSubscription);
    this.initializeTranslationStrings();
    this.initializeColumnsConfiguration();
    this.createForm();
    const suscription = this.form.valueChanges.pipe(
      debounceTime(this.ms),
      filter(response => response.filterBy)
      ).subscribe(response => {
      this.pageDescriptor.deleteAllFilters();
      if (response.search) {
        this.pageDescriptor.addOrUpdateFilters([
          {
            member: response.filterBy,
            value: response.search,
            operator: FilterOperator.Contains
          }
        ]);
      }
      this.reloadDataView();
    })
    this.entitySubscriptions.push(suscription);
  }

  reloadDataView(): void {
    this.recoverViewData();
  }

  public openCreateCountry(): void {
    this.dialogs.createEditCountry();
  }

  public openEditCountry(event: Event, country: Country): void {
    event.stopPropagation()
    this.dialogs.createEditCountry(country);
  }

  public goToRegionsList(country: Country): void {
    this.navigateTo('/support/Aieps/regions', { id: country.id, keyName: country.keyName });
  }

  public removeCountry(event, country: Country): void {
    event.stopPropagation()
    this.dialogs.confirmation('dialog.removeCountry', 'dialog.removeCountryMessage')
      .then((confirmation) => {
        if (confirmation) {
          const subscription = this.api.countries.deleteCountry(country.id)
            .subscribe((success) => {
              this.notifications.success(this.countryDeletedSuccessString);
              this.communication.notifyReloadViewData();
            }, (error) => {
              this.notifications.error(this.countryDeletedErrorString);
            });
          this.entitySubscriptions.push(subscription);
        }
      });
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
    const countriesSubscription = this.api.countries.getCountries(this.pageDescriptor)
      .subscribe((response) => {
        this.countries = response;
      });
    this.entitySubscriptions.push(countriesSubscription);
  }

  private initializeTranslationStrings(): void {
    const translationsSubscription = this.translate.get([
      'country.name',
      'country.regions',
      'notification.countryDeletedSuccess',
      'notification.countryDeletedError'
    ]).subscribe(translations => {
      this.nameHeaderString = translations['country.name'];
      this.regionsHeaderString = translations['country.regions'];
      this.countryDeletedSuccessString = translations['notification.countryDeletedSuccess'];
      this.countryDeletedErrorString = translations['notification.countryDeletedError'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }

  private initializeColumnsConfiguration(): void {
    this.columnsConfiguration = [
      { columnDef: 'keyName', header: this.nameHeaderString, sortField: 'keyName', tooltipAtLength: 56 },
      { columnDef: 'regionsCount', header: this.regionsHeaderString, sortField: 'regionsCount', tooltipAtLength: 50 }
    ];
  }

  private createForm() {
    this.form = new UntypedFormGroup({
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl(null)
    })
  }

}

