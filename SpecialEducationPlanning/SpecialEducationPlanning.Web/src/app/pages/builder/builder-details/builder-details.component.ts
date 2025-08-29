import { Component, NgZone, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { ApiService } from '../../../core/api/api.service';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { FilterDescriptor, FilterOperator } from '../../../core/services/url-parser/filter-descriptor.model';
import { PageDescriptor } from '../../../core/services/url-parser/page-descriptor.model';
import { SortDescriptor, SortDirection } from '../../../core/services/url-parser/sort-descriptor.model';
import { ComponentReloadData } from '../../../shared/base-classes/reload-data-view';
import { PlanPreviewService } from '../../../shared/components/dialogs/plan-preview/plan-preview.service';
import { CustomerNotesComponent } from '../../../shared/components/molecules/customer-notes/customer-notes.component';
import {
  CustomerContainerLeftHandSideComponent
} from '../../../shared/components/organisms/customer-container-left-hand-side/customer-container-left-hand-side.component';
import {
  AssignPlanDialogService
} from '../../../shared/components/organisms/dialogs/assign-plan/assign-plan-dialog.service';
import { TableColumnConfig, TableRecords } from '../../../shared/components/organisms/table/table.types';
import { ComponentCanDeactivate } from '../../../shared/guards/pending-changes.guard';
import { EducationToolType, PlanStateEnum } from '../../../shared/models/app-enums';
import { EnvelopeResponse } from 'src/app/core/services/url-parser/envelope-response.interface';
import { Builder } from '../../../shared/models/builder';
import { Plan } from '../../../shared/models/plan';
import { BuilderGeneralComponent } from '../builder-general/builder-general.component';
import { UntypedFormGroup } from '@angular/forms';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { PlanPreviewComponentData } from '../../../shared/components/dialogs/plan-preview/plan-preview.model';
import { EducationToolService } from '../../../core/Education-tool/Education-tool.service';

@Component({
  selector: 'tdp-builder-details',
  templateUrl: './builder-details.component.html',
  styleUrls: ['./builder-details.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BuilderDetailsComponent extends BuilderGeneralComponent implements OnInit, ComponentCanDeactivate, ComponentReloadData {
  public originalBuilder: Builder;
  public tablePlans: TableRecords<Plan>;
  public archivedTablePlans: TableRecords<Plan>;
  public builderId: number;
  public builderNotesForm: UntypedFormGroup;
  public builderNotesValue: string;
  public pageSize: number = 7;
  public planColumnNames: TableColumnConfig[] = [];

  // Translation strings
  private planId: string;
  private endUser: string;
  private EducationerName: string;
  private versionId: string;
  private lastUpdated: string;
  private assignBuilderSuccessMessage: string;
  private assignBuilderErrorMessage: string;
  private unarchivePlanError: string;
  private unarchivePlanSuccess: string;

  @ViewChild(CustomerContainerLeftHandSideComponent) leftHandsideContainerForm: CustomerContainerLeftHandSideComponent;
  @ViewChild(CustomerNotesComponent) customerNotes: CustomerNotesComponent;

  protected filterDescriptor: FilterDescriptor;

  protected pageDescriptor: PageDescriptor;
  protected archivedPlansDescriptor: PageDescriptor;
  private sortDescending: SortDescriptor;

  EducationOrigin: EducationToolType;

  constructor(
    public route: ActivatedRoute,
    public ngZone: NgZone,
    public api: ApiService,
    public notifications: NotificationsService,
    public dialogs: DialogsService,
    public dialogsPlanPreview: PlanPreviewService,
    public assignPlanDialog: AssignPlanDialogService,
    public translate: TranslateService,
    public communications: CommunicationService,
    public EducationToolService: EducationToolService,
    public planDetailsService: PlanDetailsService
  ) {
    super(api, notifications, dialogs, translate, communications);
    this.sortDescending = new SortDescriptor('UpdatedDate', SortDirection.Descending);
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
    this.createBuilderNotesForm();
    this.tablePlans = { data: [] };
    this.archivedTablePlans = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.archivedPlansDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.archivedPlansDescriptor.setPagination(0, this.pageSize);
    this.loadBuilder();
  }

  hasChanges(): boolean {
    return this.leftHandsideContainerForm?.customerForm?.customerInfoForm?.dirty || this.customerNotes?.entityForm?.dirty;
  }

  public loadActivePlansByBuidler() {
    const plansSubscription = this.getPlansByBuilder(this.builderId).subscribe((result) => {
      this.tablePlans = result;
    });
    this.entitySubscriptions.push(plansSubscription)
  }

  public loadArchivedPlansByBuidler() {
    const plansSubscription = this.getArchivedPlansByBuilder().subscribe(result => {
      this.archivedTablePlans = result;
    });
    this.entitySubscriptions.push(plansSubscription)
  }

  public getPlansByBuilder(builderId: any) {
    const builderIdFilter = new FilterDescriptor('builderId', FilterOperator.IsEqualTo, builderId);
    this.pageDescriptor.addOrUpdateFilter(builderIdFilter);
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    return this.api.plans.getPlansFiltered(this.pageDescriptor);
  }

  public getArchivedPlansByBuilder() {
    this.archivedPlansDescriptor.addFilter(new FilterDescriptor('planState', FilterOperator.IsEqualTo, '1'))
    this.archivedPlansDescriptor.addFilter(new FilterDescriptor('builderId', FilterOperator.IsEqualTo, this.builderId.toString()));
    this.archivedPlansDescriptor.addFilter(new FilterDescriptor('EducationOrigin', FilterOperator.IsEqualTo, EducationToolType.FUSION));

    this.archivedPlansDescriptor.addOrUpdateSort(this.sortDescending);
    return this.api.plans.getPlansFiltered(this.archivedPlansDescriptor);
  }

  public reloadDataView(): void {
    if (this.tablePlans.data.length) {
      this.loadActivePlansByBuidler();
    }
    if (this.archivedTablePlans.data.length) {
      this.loadArchivedPlansByBuidler();
    }
  }

  public pageChanged(event: PageEvent) {
    this.pageDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.loadActivePlansByBuidler();
  }

  public pageChangedArchivedPlans(event: PageEvent) {
    this.archivedPlansDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.loadArchivedPlansByBuidler();
  }

  public sortChanged(event: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(event);
    this.loadActivePlansByBuidler();
  }

  public sortChangedArchivedPlans(event: SortDescriptor) {
    this.archivedPlansDescriptor.deleteAllSorts();
    this.archivedPlansDescriptor.addOrUpdateSort(event);
    this.loadArchivedPlansByBuidler();
  }

  public goToPlanDetails(plan: Plan) {
    this.navigateTo('/plan/' + plan.id);
  }

  public openPreviewDialog(event: any, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    if (plan.masterVersionId) {
      const data: PlanPreviewComponentData = {
        versionId: plan.masterVersionId,
        plan: plan,
        showButton: true
      }
      this.dialogsPlanPreview.planPreview(data);
    } else {
      this.dialogs.simpleInformation('dialog.emptyPlanTitle', 'dialog.emptyPlanMsg');
    }
  }

  public openPlanInEducationTool(event: MouseEvent, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    this.goToPlanDetails(plan);
    this.EducationToolService.openPlan(plan, plan.EducationOrigin);
  }

  public openTransferSinglePlanDialog(event: any, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.transferSinglePlan('70em', plan);
  }

  public restoreArchivedPlan(event: MouseEvent, plan: Plan) {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.confirmation('dialog.unarchivePlan', 'dialog.unarchiveMessage')
      .then((confirmation) => {
        if (confirmation) {
          const planStateSubscription = this.api.plans.changePlanState(plan.id, PlanStateEnum.Active)
            .subscribe(
              (response) => {
                this.notifications.success(this.unarchivePlanSuccess);
                plan.planState = response.planState;
                this.reloadDataView();
              },
              (error) => {
                this.notifications.error(this.unarchivePlanError);
              }
            );
          this.entitySubscriptions.push(planStateSubscription);
        }
      });

  }

  public openUnassignedPlanDialog(): void {
    const pageDescriptor = new PageDescriptor();
    pageDescriptor.setPagination(0, this.pageSize);
    pageDescriptor.addOrUpdateFilter(new FilterDescriptor('builderId', FilterOperator.IsEqualTo, null));
    const unassignedPlansSubscription = this.api.plans.getPlansFiltered(pageDescriptor)
      .subscribe((plansFilteredResponse: EnvelopeResponse<Plan>) => {
        if (plansFilteredResponse.data.length) {
          this.assignPlanDialog.unassignedPlans(plansFilteredResponse)
            .then((selectedPlan: Plan) => {
              if (selectedPlan) {
                const assignSubscription = this.api.plans.assignBuilderToPlan(selectedPlan.id, this.originalBuilder.id)
                  .subscribe((response: Plan) => {
                    setTimeout(() => {
                      this.reloadDataView();
                      this.notifications.success(this.assignBuilderSuccessMessage);
                    }, 750);
                  }, (error) => {
                    this.notifications.error(this.assignBuilderErrorMessage);
                  });
                this.entitySubscriptions.push(assignSubscription);
              }
            });
        } else {
          this.dialogs.simpleInformation('dialog.noUnassignedPlansFoundTitle', 'dialog.noUnassignedPlansFoundMsg');
        }
      });
    this.entitySubscriptions.push(unassignedPlansSubscription);
  }

  public updateInfo(builder: Builder) {
    this.api.builders.updateBuilder(builder).subscribe(result => {
      return result
    })
  }

  public createBuilderNotesForm() {
    this.builderNotesForm = this.formBuilder.group({
      builderNotes: [null]
    })
  }

  public initializeBuilderNotes() {
    this.builderNotesForm = this.formBuilder.group({
      builderNotes: [this.builderNotesValue]
    })
  }

  public updateBuilderNotes(event: any) {
    this.originalBuilder.notes = event.note;
    this.api.builders.updateBuilder(this.originalBuilder).subscribe();
  }

  public loadBuilder() {
    const routeSubscription = this.route.params.subscribe((params) => {
      this.builderId = +params['id']
      const builderSubscription = this.api.builders.getBuilder(this.builderId).subscribe(builder => {
        this.originalBuilder = builder;
        this.loadActivePlansByBuidler();
        this.loadArchivedPlansByBuidler();
        this.builderNotesValue = this.originalBuilder.notes;
        this.initializeBuilderNotes();
      })
      this.entitySubscriptions.push(builderSubscription)
    })
    this.entitySubscriptions.push(routeSubscription);
  }

  public openCreatePlanModal() {
    this.planDetailsService.setTradeCustomer(this.originalBuilder);
    this.dialogs.openCreatePlanModal();
  }

  private getEducationerFullName(record) {
    let name = record.Educationer?.firstName ? record.Educationer.firstName : '';
    let surname = record.Educationer?.surname ? record.Educationer.surname : '';
    let fullName = (name || surname) ? `${name} ${surname}` : '-';
    return fullName;
  }

  private  initializeColumnConfiguration() {
    this.planColumnNames = [
      { columnDef: 'planCode', header: this.planId, sortField: 'PlanCode', tooltipAtLength: 13 },
      { columnDef: 'endUser', header: this.endUser, field: 'endUser.fullName', sortField: 'EndUser.Surname', tooltipAtLength: 15 },
      { columnDef: 'Educationer', header: this.EducationerName, callback: (record: any) => this.getEducationerFullName(record), sortField: 'Educationer.Surname', tooltipAtLength: 12 },
      { columnDef: 'EducationOrigin', header: this.EducationOrigin, sortField: 'EducationOrigin', isDate: false },
      { columnDef: 'updatedDate', header: this.lastUpdated, sortField: 'UpdatedDate', isDate: true }
    ];
  }

  protected initializeTranslationStrings(): void {
    const translationsSubscription = this.translate.get([
      'plan.planId',
      'plan.endUser',
      'plan.Educationer',
      'plan.versionId',
      'plan.lastUpdated',
      'plan.EducationOrigin',
      'notification.assignBuilderSuccess',
      'notification.assignBuilderError',
      'notification.archivePlanError',
      'notification.unarchivePlanSuccess'
    ]).subscribe(translations => {
      this.planId = translations['plan.planId'];
      this.endUser = translations['plan.endUser'];
      this.EducationerName = translations['plan.Educationer'];
      this.versionId = translations['plan.versionId'];
      this.lastUpdated = translations['plan.lastUpdated'];
      this.EducationOrigin = translations['plan.EducationOrigin'];
      this.assignBuilderSuccessMessage = translations['notification.assignBuilderSuccess'];
      this.assignBuilderErrorMessage = translations['notification.assignBuilderError'];
      this.unarchivePlanError = translations['notification.archivePlanError'];
      this.unarchivePlanSuccess = translations['notification.unarchivePlanSuccess'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }
}

