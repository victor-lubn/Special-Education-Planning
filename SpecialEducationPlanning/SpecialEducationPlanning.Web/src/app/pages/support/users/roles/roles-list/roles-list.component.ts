import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../../../../core/api/api.service';
import { CommunicationService } from '../../../../../core/services/communication/communication.service';
import { ComponentReloadData } from '../../../../../shared/base-classes/reload-data-view';
import { Role } from '../../../../../shared/models/role';
import { ListComponent } from '../../../../../shared/base-classes/list-component';
import { DialogsService } from '../../../../../core/services/dialogs/dialogs.service';
import { FilterOperator } from '../../../../../core/services/url-parser/filter-descriptor.model';
import { AppEntitiesEnum } from '../../../../../shared/models/app-enums';
import { SortingFilteringItemsService } from '../../../../../core/services/sorting-filtering-items/sortingFilteringItems.service';
import { SelectOptionInterface } from '../../../../../shared/components/atoms/select/select.component';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime } from 'rxjs/operators';
import { PageEvent } from '@angular/material/paginator';
import { SortDescriptor } from '../../../../../core/services/url-parser/sort-descriptor.model';
import { TableColumnConfig } from 'src/app/shared/components/organisms/table/table.types';

@Component({
  selector: 'tdp-roles-list',
  templateUrl: './roles-list.component.html',
  styleUrls: ['./roles-list.component.scss']
})
export class RolesListComponent extends ListComponent<Role> implements OnInit, ComponentReloadData {
  options: SelectOptionInterface[]
  form: UntypedFormGroup;
  rolesString: string
  columnsConfiguration: TableColumnConfig[];
  public ms: number = 400;

  constructor(
    private api: ApiService,
    private communication: CommunicationService,
    private dialogs: DialogsService,
    private sortingFiltering: SortingFilteringItemsService,
    private translate: TranslateService,
  ) {
    super();
  }

  ngOnInit() {
    this.createForm()
    this.initializeTranslationStrings()
    this.initializeColumnConfiguration()
    this.sortingFiltering.getOptions(AppEntitiesEnum.role).then(options$ => {
      const subscription = options$.subscribe(options => {
        this.options = options;
      }
      );
      this.entitySubscriptions.push(subscription);
    });
    this.pageDescriptor.setPagination(0, this.defaultPageSize);
    this.recoverViewData();
    const subscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(subscription);
    const suscription = this.form.valueChanges.pipe(debounceTime(this.ms)).subscribe(response => {
      this.pageDescriptor.addOrUpdateFilter(
        {
          member: response.filterBy,
          value: response.search,
          operator: FilterOperator.Contains
        }
      );
      this.recoverViewData()
      this.reloadDataView();
    })
    this.entitySubscriptions.push(suscription);
  }

  reloadDataView(): void {
    this.recoverViewData();
  }

  private createForm() {
    this.form = new UntypedFormGroup({
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl(null)
    })
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

  public openCreateNewRole(): void {
    this.dialogs.createEditRole();
  }

  openEditRole(event, role: Role) {
    event.stopPropagation()
    this.dialogs.createEditRole(role);
  }

  protected recoverViewData(): void {
    const subscription = this.api.roles.getRoles(this.pageDescriptor)
      .subscribe((response) => {
        this.entityEnvelopeResponse = response;
      });
    this.entitySubscriptions.push(subscription);
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'rolesPage.role'
    ]).subscribe(translations => {
      this.rolesString = translations['rolesPage.role'];
    })
    this.entitySubscriptions.push(translationSubscription);
  }

  private initializeColumnConfiguration(): void {
    this.columnsConfiguration = [
      { columnDef: 'name', header: this.rolesString, sortField: 'name' }
    ];
  }
}
