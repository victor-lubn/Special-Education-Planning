import { Component, OnInit } from '@angular/core';

import { User } from '../../../../shared/models/user';
import { ComponentReloadData } from '../../../../shared/base-classes/reload-data-view';
import { ApiService } from '../../../../core/api/api.service';
import { ListComponent } from '../../../../shared/base-classes/list-component';
import { AppEntitiesEnum } from '../../../../shared/models/app-enums';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { TableColumnConfig, TableRecords } from '../../../../shared/components/organisms/table/table.types';
import { PageDescriptor } from '../../../../core/services/url-parser/page-descriptor.model';
import { PageEvent } from '@angular/material/paginator';
import { SortDescriptor } from '../../../../core/services/url-parser/sort-descriptor.model';
import { TranslateService } from '@ngx-translate/core';
import { map } from 'rxjs/operators';
import { SelectOptionInterface } from '../../../../shared/components/atoms/select/select.component';
import { debounceTime } from 'rxjs/operators';
import { FilterOperator } from '../../../../core/services/url-parser/filter-descriptor.model';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import {
  SortingFilteringItemsService
} from '../../../../core/services/sorting-filtering-items/sortingFilteringItems.service';

@Component({
  selector: 'tdp-user-list',
  templateUrl: 'user-list.component.html',
  styleUrls: ['user-list.component.scss']
})
export class UserListComponent extends ListComponent<User> implements OnInit, ComponentReloadData {
  options: SelectOptionInterface[];
  form: UntypedFormGroup;

  showRolesButton: boolean;
  userName: string;
  userRole: string;
  userUpn: string;
  userAiepCode: string;
  userAiepName: string;
  columnsConfiguration: TableColumnConfig[];

  public users: TableRecords<User & { role?: string }> = {
    data: []
  };
  public ms: number = 400;

  readonly pageSize: number = 7;

  constructor(
    private api: ApiService,
    private translate: TranslateService,
    private userInfo: UserInfoService,
    private sortingFiltering: SortingFilteringItemsService,
  ) {
    super();
    this.showRolesButton = false;
    this.users = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
  }

  ngOnInit() {
    this.showRolesButton = this.userInfo.hasPermission('Role_Management');
    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
    this.getFilteringOptions();
    this.recoverViewData();
    this.createForm();
    const subscription = this.form.valueChanges.pipe(debounceTime(this.ms)).subscribe(response => {
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
    });
    this.entitySubscriptions.push(subscription);
  }

  initializeColumnConfiguration(): void {
    this.columnsConfiguration = [
      { columnDef: 'surname', header: this.userName, sortField: 'surname', tooltipAtLength: 20 },
      { columnDef: 'role', header: this.userRole, tooltipAtLength: 15 },
      { columnDef: 'uniqueIdentifier', header: this.userUpn, sortField: 'uniqueIdentifier', tooltipAtLength: 28 },
      { columnDef: 'Aiep.AiepCode', header: this.userAiepCode, sortField: 'Aiep.AiepCode' },
      { columnDef: 'Aiep.name', header: this.userAiepName, sortField: 'Aiep.name', tooltipAtLength: 17 }
    ];
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

  public goToUsersDetails(user: User): void {
    this.navigateTo('/support/users/' + user.id);
  }

  public goToCreateNewUser(): void {
    this.navigateTo('/support/users/new');
  }

  public goToRolesList(): void {
    this.navigateTo('/support/users/roles');
  }

  protected recoverViewData(): void {
    const subscription = this.api.users.getUsersFiltered(this.pageDescriptor)
      .pipe(map(userResponse => {
        return {
          ...userResponse,
          data: userResponse.data.map(user => {
            return {
              ...user,
              surname: user.firstName !== null ? `${user.firstName} ${user.surname}` : `${user.surname}`,
              role: `${user.userRoles[0].role.name}`
            };
          })
        };
      })).subscribe((response) => {
        this.users = response;
      });
    this.entitySubscriptions.push(subscription);
  }

  private getFilteringOptions() {
    this.sortingFiltering.getOptions(AppEntitiesEnum.user).then(options$ => {
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
    });
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'user.name',
      'user.role',
      'user.upn',
      'user.AiepCode',
      'user.AiepName'
    ]).subscribe(translations => {
      this.userName = translations['user.name'];
      this.userRole = translations['user.role'];
      this.userUpn = translations['user.upn'];
      this.userAiepCode = translations['user.AiepCode'];
      this.userAiepName = translations['user.AiepName'];
    });
    this.entitySubscriptions.push(translationSubscription);
  }

}

