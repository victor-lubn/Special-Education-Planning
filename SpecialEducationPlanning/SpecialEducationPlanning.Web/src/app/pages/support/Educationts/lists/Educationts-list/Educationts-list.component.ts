import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { debounceTime } from 'rxjs/operators';
import { FilterOperator } from 'src/app/core/services/url-parser/filter-descriptor.model';
import { PageDescriptor } from 'src/app/core/services/url-parser/page-descriptor.model';
import { SortDescriptor } from 'src/app/core/services/url-parser/sort-descriptor.model';
import { TableColumnConfig, TableRecords } from 'src/app/shared/components/organisms/table/table.types';
import { ApiService } from '../../../../../core/api/api.service';
import { CommunicationService } from '../../../../../core/services/communication/communication.service';
import { SortingFilteringItemsService } from '../../../../../core/services/sorting-filtering-items/sortingFilteringItems.service';
import { ListComponent } from '../../../../../shared/base-classes/list-component';
import { ComponentReloadData } from '../../../../../shared/base-classes/reload-data-view';
import { SelectOptionInterface } from '../../../../../shared/components/atoms/select/select.component';
import { AppEntitiesEnum } from '../../../../../shared/models/app-enums';
import { Aiep } from '../../../../../shared/models/Aiep.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-Aieps-list',
  templateUrl: 'Aieps-list.component.html',
  styleUrls: ['Aieps-list.component.scss']
})
export class AiepsListComponent extends ListComponent<Aiep> implements OnInit, ComponentReloadData {
  options: SelectOptionInterface[];
  form: UntypedFormGroup;
  public entityForm: UntypedFormGroup;
  public ms: number = 400;
  public columnsConfiguration: TableColumnConfig[] = [];
  readonly pageSize: number = 7;

  public Aieps: TableRecords<Aiep>;

  private AiepCode: string;
  private name: string;
  private area: string;
  private lastUpdate: string;

  constructor(
    private api: ApiService,
    private communication: CommunicationService,
    private sortingFiltering: SortingFilteringItemsService,
    public translate: TranslateService,
  ) {
    super();
    this.Aieps = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
  }

  ngOnInit() {
    this.getFilteringOptions();
    this.recoverViewData();
    this.initializeReloadViewDataSubscription();
    this.createForm();
    const suscription = this.form.valueChanges.pipe(debounceTime(this.ms)).subscribe(response => {
      if (!response.filterBy) {
        return;
      }
      this.pageDescriptor.deleteAllFilters();
      this.pageDescriptor.addOrUpdateFilters([
        {
          member: response.filterBy,
          value: response.search,
          operator: FilterOperator.Contains
        }
      ]);
      this.reloadDataView();
    })
    this.entitySubscriptions.push(suscription);
    this.initializeTranslationStrings();
    this.initializeColumnsConfiguration();
  }


  private initializeReloadViewDataSubscription() {
    const reloadViewDataSubscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(reloadViewDataSubscription);
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

  public goToAiepDetails(Aiep: Aiep): void {
    this.navigateTo('/support/Aieps/' + Aiep.id);
  }

  public goToCountriesList(): void {
    this.navigateTo('/support/Aieps/countries');
  }

  public goToNewAiepPage(): void {
    this.navigateTo('/support/Aieps/new');
  }

  protected recoverViewData(): void {
    const AiepsSubscription = this.api.Aieps.getAieps(this.pageDescriptor)
      .subscribe((response) => {
        this.Aieps = response;
      });
    this.entitySubscriptions.push(AiepsSubscription);
  }

  private getFilteringOptions() {
    this.sortingFiltering.getOptions(AppEntitiesEnum.Aiep).then(options$ => {
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

  private initializeColumnsConfiguration() {
    this.columnsConfiguration = [
      { columnDef: 'AiepCode', header: this.AiepCode, sortField: 'AiepCode' },
      { columnDef: 'name', header: this.name, sortField: 'name', tooltipAtLength: 39 },
      { columnDef: 'area.keyName', header: this.area, sortField: 'area.keyName', tooltipAtLength: 30 },
      { columnDef: 'updatedDate', header: this.lastUpdate, sortField: 'updatedDate', isDate: true },
    ];
  }

  private initializeTranslationStrings() {
    const translationsSubscription = this.translate.get([
      'Aiep.AiepCode',
      'Aiep.name',
      'Aiep.area',
      'Aiep.lastUpdate'
    ]).subscribe(translations => {
      this.AiepCode = translations['Aiep.AiepCode'];
      this.name = translations['Aiep.name'];
      this.area = translations['Aiep.area'];
      this.lastUpdate = translations['Aiep.lastUpdate'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }
}

