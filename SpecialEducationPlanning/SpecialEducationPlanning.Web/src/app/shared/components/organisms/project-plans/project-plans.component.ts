import { Component, ElementRef, Input, NgZone, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TableColumnConfig, TableRecords } from '../table/table.types';
import { Plan } from 'src/app/shared/models/plan';
import { GlobalSearchFilter } from 'src/app/shared/models/global-search-filter';
import { CdkScrollable, ScrollDispatcher } from '@angular/cdk/scrolling';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { ApiService } from 'src/app/core/api/api.service';
import { EducationToolType, PlanStateEnum } from 'src/app/shared/models/app-enums';
import { NotificationsService } from 'angular2-notifications';
import { BaseEntity } from 'src/app/shared/base-classes/base-entity';
import { Project } from 'src/app/shared/models/project';
import { Router } from '@angular/router';
import { EducationToolService } from '../../../../core/Education-tool/Education-tool.service';

@Component({
  selector: 'tdp-project-plans',
  templateUrl: './project-plans.component.html',
  styleUrls: ['./project-plans.component.scss'],
})
export class ProjectPlansComponent extends BaseEntity implements OnInit, OnChanges {

  @Input() project: Project;
  @Input() plans: Plan[] = [];
  public plansRecords: TableRecords<any> = { data: [] };
  public planColumnNames: TableColumnConfig[] = [];
  public globalFilter: GlobalSearchFilter;
  public minimizeHeader = false;
  public cadPlanNo: string;
  public housingSpec: string;
  public housingType: string = '';
  public status: string = '';
  public active: string = '';
  public archived: string = '';
  public planStateEnum = PlanStateEnum;
  protected archivePlanSuccess: string = '';
  protected archivePlanError: string = '';
  private unarchivePlanSuccess: string = '';
  private unarchivePlanError: string = '';
  EducationToolType = EducationToolType;
  @ViewChild("header", { static: false }) header: ElementRef;

  constructor(
    private translate: TranslateService,
    private scrollDispatcher: ScrollDispatcher,
    private zone: NgZone,
    protected dialogs: DialogsService,
    protected api: ApiService,
    protected notifications: NotificationsService,
    protected EducationToolService: EducationToolService,
    private router: Router
  ) {
    super();
    this.plansRecords = { data: [] };
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
    this.updatePlansRecords(this.plans);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.plans && changes.plans.currentValue) {
      this.updatePlansRecords(this.plans);
    }
  }

  private updatePlansRecords(plans: Plan[]): void {
    this.plansRecords = {
      data: plans
        .filter(plan => !plan.isTemplate)
        .map((plan, index) => ({
        ...plan,
        cadPlanNo: plan.planCode,
        housingSpec: plan.housingSpecificationTemplatesModel?.housingSpecificationModel?.name
        ?? plan.housingSpecificationTemplatesModel?.housingSpecification?.name
        ?? plan.housingTypeModel?.housingSpecificationModel?.name,
        housingType: plan.planType,
        status: plan.planState === 1 ? this.archived : this.active
      }))
    };
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'projectPlans.cadPlanNo',
      'projectPlans.housingSpec',
      'projectPlans.housingType',
      'projectPlans.status',
      'projectPlans.active',
      'projectPlans.archived',
      'notification.archivePlanSuccess',
      'notification.archivePlanError',
      'notification.unarchivePlanSuccess',
      'notification.unarchivePlanError'
    ]).subscribe((translations) => {
      this.cadPlanNo = translations['projectPlans.cadPlanNo'];
      this.housingSpec = translations['projectPlans.housingSpec'];
      this.housingType = translations['projectPlans.housingType'];
      this.status = translations['projectPlans.status'];
      this.active = translations['projectPlans.active'];
      this.archived = translations['projectPlans.archived'];
      this.archivePlanSuccess = translations['notification.archivePlanSuccess'];
      this.archivePlanError = translations['notification.archivePlanError'];
      this.unarchivePlanSuccess = translations['notification.unarchivePlanSuccess'];
      this.unarchivePlanError = translations['notification.unarchivePlanError'];
    });
  }

  private initializeColumnConfiguration() {
    this.planColumnNames = [
      { columnDef: 'cadPlanNo', header: this.cadPlanNo, tooltipAtLength: 20 },
      { columnDef: 'housingSpec', header: this.housingSpec, tooltipAtLength: 20 },
      { columnDef: 'housingType', header: this.housingType, tooltipAtLength: 20 },
      { columnDef: 'status', header: this.status, tooltipAtLength: 20 },
    ]
  }

  ngAfterViewInit() {
    this.scrollDispatcher.scrolled().subscribe((cdk: CdkScrollable) => {
      this.zone.run(() => {
        const scrollPosition = cdk?.getElementRef().nativeElement.scrollTop;
        if (this.header && scrollPosition > this.header.nativeElement.offsetHeight) {
          this.minimizeHeader = true;
        }
      });
    });
  }

  editInEducationTool(event: MouseEvent, plan: Plan) {
    event.stopPropagation();
    event.preventDefault();
    if (plan && plan.masterVersion) {
      this.EducationToolService.openVersionInEducationTool(plan.masterVersion, plan.builderId, plan.EducationOrigin);
    } else {
      this.dialogs.simpleInformation('dialog.emptyPlanTitle', 'dialog.emptyPlanMsg');
    }
  }

  publishPlan(event: MouseEvent, record: any): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.openTenderPackPlanPublishModal(record);
  }

  publishAllPlans() {
    this.dialogs.openTenderPackPlanPublishModal(this.project.id);
  }

  private updatePlanState(response: Plan, planId: number): void {
    const record = this.plansRecords.data.find(p => p.id === planId);
    if (record) {
      record.planState = response.planState;
      record.status = response.planState === 1 ? this.archived : this.active;
    }
  }

  public openArchiveDialog(event: MouseEvent, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.confirmation('dialog.archivePlan', 'dialog.archiveMessage')
      .then((confirmation) => {
        if (confirmation) {
          const planStateSubscription = this.api.plans.changePlanState(plan.id, this.planStateEnum.Archived)
            .subscribe(
              (response) => {
                this.notifications.success(this.archivePlanSuccess);
                this.updatePlanState(response, plan.id);
              },
              (error) => {
                this.notifications.error(this.archivePlanError);
              }
            );
          this.entitySubscriptions.push(planStateSubscription);
        }
      });
  }

  public openRestoreDialog(event: MouseEvent, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.confirmation('dialog.unarchivePlan', 'dialog.unarchiveMessage')
    .then((confirmation) => {
      if (confirmation) {
        const planStateSubscription = this.api.plans.changePlanState(plan.id, this.planStateEnum.Active)
          .subscribe(
            (response) => {
              this.notifications.success(this.unarchivePlanSuccess);
              this.updatePlanState(response, plan.id);
            },
            (error) => {
              this.notifications.error(this.unarchivePlanError);
            }
          );
        this.entitySubscriptions.push(planStateSubscription);
      }
    });
  }

  public onPlanClick(plan: Plan): void {
    if(plan.planState === this.planStateEnum.Active) {
      localStorage.setItem('projectHousingSpecifications', JSON.stringify(this.project.housingSpecifications));
      this.router.navigate([`/plan/${plan.id}`], { queryParams: { isTenderPack: true }});
    }
  }
}

