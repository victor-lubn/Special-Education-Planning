import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { ApiService } from 'src/app/core/api/api.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { EnvelopeResponse } from 'src/app/core/services/url-parser/envelope-response.interface';
import { FilterDescriptor, FilterOperator } from 'src/app/core/services/url-parser/filter-descriptor.model';
import { PageDescriptor } from 'src/app/core/services/url-parser/page-descriptor.model';
import { SortDescriptor, SortDirection } from 'src/app/core/services/url-parser/sort-descriptor.model';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { Plan } from 'src/app/shared/models/plan';
import { TableService } from '../../table/table.service';
import { TableColumnConfig, TableRecords } from '../../table/table.types';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-assign-plan-dialog',
  templateUrl: './assign-plan-dialog.component.html',
  styleUrls: ['./assign-plan-dialog.component.scss'],
  providers: [TableService]
})
export class AssignPlanDialogComponent extends BaseComponent implements OnInit {

  public selected: any;
  public tablePlans: TableRecords<Plan>;

  public pageSize = 7;
  public columnsConfiguration: TableColumnConfig[] = [];
  protected pageDescriptor: PageDescriptor;
  private sortDescending: SortDescriptor;

  private planId: string;
  private endUser: string;
  private EducationerName: string;
  private lastUpdated: string;

  constructor(
    private tableService: TableService,
    private dialogs: DialogsService,
    private api: ApiService,
    public translate: TranslateService,
    private _dialogRef: MatDialogRef<AssignPlanDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: {
      tablePlans: TableRecords<Plan>,
    }
  ) {
    super();
    this._dialogRef.disableClose = true;
    this.tablePlans = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.addOrUpdateFilter(new FilterDescriptor('builderId', FilterOperator.IsEqualTo, null));
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.sortDescending = new SortDescriptor('UpdatedDate', SortDirection.Descending)
  }

  ngOnInit(): void {
    this.tablePlans = this._data.tablePlans;
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
    const selectedPlanSubscription = this.tableService.onSelectedRowChanged.subscribe(
      (plan: Plan) => {
        this.selected = plan;
      }
    );
    this.entitySubscriptions.push(selectedPlanSubscription);
  }

  loadPlansFiltered(): void {
    const unassignedPlansSubscription = this.api.plans.getPlansFiltered(this.pageDescriptor)
      .subscribe((plansFilteredResponse: EnvelopeResponse<Plan>) => {
        if (plansFilteredResponse.data.length) {
          this.tablePlans = plansFilteredResponse;
        }
      })
    this.entitySubscriptions.push(unassignedPlansSubscription);
  }

  onAssignPlan() {
    this.dialogs.confirmation(
      'dialog.unassignedPlanDialogTitle',
      'dialog.unassignedPlanDialogMessage',
      'booleanResponse.no',
      'booleanResponse.yes',
      '400px'
    ).then((confirmation: boolean) => {
      if (confirmation) {
        this._dialogRef.close(this.selected);
      }
    });
  }

  onCancel() {
    this.dialogs.confirmation(
      'dialog.unassignedPlanDialogTitle',
      'dialog.unassignedPlanCancelMessage',
      'booleanResponse.no',
      'booleanResponse.yes',
      '400px'
    ).then((confirmation: boolean) => {
      if (confirmation) {
        this._dialogRef.close(null);
      }
    });
  }

  public pageChanged(event: PageEvent) {
    this.pageDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.loadPlansFiltered();
  }

  public sortChanged(event: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(event);
    this.loadPlansFiltered();
  }

  private getEducationerFullName(record) {
    let name = record.Educationer?.firstName ? record.Educationer.firstName : '';
    let surname = record.Educationer?.surname ? record.Educationer.surname : '';
    let fullName = (name || surname) ? `${name} ${surname}` : '-';
    return fullName;
  }

  private  initializeColumnConfiguration() {
    this.columnsConfiguration = [
      { columnDef: 'planCode', header: this.planId, sortField: 'PlanCode', tooltipAtLength: 23 },
      { columnDef: 'endUser', header: this.endUser, field: 'endUser.fullName', sortField: 'EndUser.Surname', tooltipAtLength: 13 },
      { columnDef: 'Educationer', header: this.EducationerName, callback: (record: any) => this.getEducationerFullName(record), sortField: 'Educationer.Surname', tooltipAtLength: 14 },
      { columnDef: 'updatedDate', header: this.lastUpdated, sortField: 'UpdatedDate', isDate: true }
    ];
  }

  protected initializeTranslationStrings(): void {
    const translationsSubscription = this.translate.get([
      'plan.planId',
      'plan.endUser',
      'plan.Educationer',
      'plan.lastUpdated'
    ]).subscribe(translations => {
      this.planId = translations['plan.planId'];
      this.endUser = translations['plan.endUser'];
      this.EducationerName = translations['plan.Educationer'];
      this.lastUpdated = translations['plan.lastUpdated'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }

}

