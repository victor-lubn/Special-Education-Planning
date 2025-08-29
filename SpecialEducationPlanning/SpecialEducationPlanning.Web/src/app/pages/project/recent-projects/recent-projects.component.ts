import {
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from "@angular/core";
import { DatePipe } from "@angular/common";
import { CdkScrollable, ScrollDispatcher } from "@angular/cdk/scrolling";
import { TranslateService } from "@ngx-translate/core";
import { ApiService } from "../../../core/api/api.service";
import { CommunicationService } from "../../../core/services/communication/communication.service";
import { TableColumnConfig, TableRecords} from "../../../shared/components/organisms/table/table.types";
import { GlobalSearchFilter } from "src/app/shared/models/global-search-filter";
import { MultiTable } from "src/app/shared/components/molecules/multi-table/types";
import { Project } from "src/app/shared/models/project";
import { SortDescriptor, SortDirection } from "src/app/core/services/url-parser/sort-descriptor.model";
import { PageDescriptor } from "src/app/core/services/url-parser/page-descriptor.model";
import { PageEvent } from "@angular/material/paginator";
import { Subscription } from "rxjs";
import { BaseComponent } from "src/app/shared/base-classes/base-component";
import { DialogsService } from "src/app/core/services/dialogs/dialogs.service";
import { PlanStateEnum } from 'src/app/shared/models/app-enums';
import { NotificationsService } from 'angular2-notifications';
import { creatioEnvUrl } from "src/environments/environment";
import { TableService } from "src/app/shared/components/organisms/table/table.service";
import { FilterDescriptor, FilterOperator } from 'src/app/core/services/url-parser/filter-descriptor.model';
import { TdpUtils } from 'src/app/core/utils';
import { ComponentReloadData } from "src/app/shared/base-classes/reload-data-view";
import { ElectronService } from "src/app/core/electron-api/electron.service";
import { PlanService } from "src/app/core/api/plan/plan.service";

@Component({
  selector: "tdp-recent-projects",
  templateUrl: "./recent-projects.component.html",
  styleUrls: ["./recent-projects.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class RecentProjectsComponent extends BaseComponent implements OnInit, OnDestroy, ComponentReloadData
{
  public tables: MultiTable[]; 
  public projects: TableRecords<Project>;
  public minimizeHeader = false;
  public pageSize: number = 7;
  public projectColumnNames: TableColumnConfig[] = [];
  public globalFilter: GlobalSearchFilter = null;
  public projectName: string;
  public projectReference: string;
  public lastUpdated: string;
  public columnsConfiguration: TableColumnConfig[];
  private pageDescriptor: PageDescriptor;
  private sortDescending: SortDescriptor;
  public planStateEnum = PlanStateEnum;
  protected archiveProjectSuccess: string = '';
  protected archiveProjectError: string = '';
  private unarchiveProjectSuccess: string = '';
  private unarchiveProjectError: string = '';
  private noProjectSearchResults: string =  '';
  private noActivePlans: string = '';
  @ViewChild("header", { static: false }) header: ElementRef;

  constructor(
    protected api: ApiService,
    protected translate: TranslateService,
    protected communication: CommunicationService,
    private scrollDispatcher: ScrollDispatcher,
    protected ngZone: NgZone,
    private zone: NgZone,
    private dialogs: DialogsService,
    protected notifications: NotificationsService,
    private tableService: TableService,
    private electronService: ElectronService,
    private planService: PlanService
  ) {
    super();
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.sortDescending = new SortDescriptor('UpdatedDate', SortDirection.Descending);
    this.projects = { data: [] };
    this.projectName = '';
    this.projectReference = '';
    this.lastUpdated = '';
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
    this.tables = [this.buildProjects({ data: [] })];
    this.loadData(); 
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);

    const globalSearchFilterSub = this.communication.subscribeToTopbarSearchChange().subscribe(globalFilter => {
      this.globalFilter = globalFilter;
      if (this.globalFilter) {
        this.applyGlobalSearchFilters(globalFilter);
      }
      this.resetView();
    });

    const reloadViewSubscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });

    this.entitySubscriptions.push(
      reloadViewSubscription,
      globalSearchFilterSub
    );
  }

  get tableHeight(): string {
    if (this.globalFilter && this.globalFilter.selected.key === "omniSearch") {
      return "700px";
    } else if (this.minimizeHeader) {
      return "650px";
    } else {
      return "600px";
    }
  }

  resetView() {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.tableService.resetCurrentTable();
    this.loadData();
  }

  reloadDataView(): void {
    if (this.projects.data.length) {
      this.loadData();
    }
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

  private applyGlobalSearchFilters(globalSearchFilter: GlobalSearchFilter): void {
      let filters: FilterDescriptor[] = [];
      switch (globalSearchFilter.selected.key) {
        case 'homeFilterBuilderForm': {
          const mappers = [
            {
              member: 'accountNumber',
              operator: FilterOperator.Contains,
              path: 'accountNumber'
            },
            {
              member: 'tradingName',
              operator: FilterOperator.Contains,
              path: 'tradingName'
            },
            {
              member: 'name',
              operator: FilterOperator.Contains,
              path: 'name'
            },
            {
              member: 'address0',
              operator: FilterOperator.Contains,
              path: 'address0'
            },
            {
              member: 'postcode',
              operator: FilterOperator.Contains,
              path: 'postcode'
            },
            {
              member: 'mobileNumber',
              operator: FilterOperator.Contains,
              path: 'mobileNumber'
            },
            {
              member: 'landlineNumber',
              operator: FilterOperator.Contains,
              path: 'landlineNumber'
            },
          ];
          filters = TdpUtils.resolveFilterDescriptors({
            filter: globalSearchFilter.selected.value,
            mappers
          });
          break;
        }
        case 'homeFilterPlanForm': {
          const mappers = [
            {
              member: 'versions.externalCode',
              operator: FilterOperator.Contains,
              path: 'versions.externalCode'
            },
            {
              member: 'endUser.surname',
              operator: FilterOperator.Contains,
              path: 'endUser.surname'
            },
            {
              member: 'endUser.address0',
              operator: FilterOperator.Contains,
              path: 'endUser.address0'
            },
            {
              member: 'endUser.postcode',
              operator: FilterOperator.Contains,
              path: 'endUser.postcode'
            },
            {
              member: 'planCode',
              operator: FilterOperator.Contains,
              path: 'planCode'
            },
            {
              member: 'planName',
              operator: FilterOperator.Contains,
              path: 'planName'
            },
            {
              member: 'cadFilePlanId',
              operator: FilterOperator.Contains,
              path: 'cadFilePlanId'
            },
            {
              member: 'EducationerId',
              operator: FilterOperator.IsEqualTo,
              path: 'EducationerId'
            },
            {
              member: 'builderId',
              operator: FilterOperator.IsEqualTo,
              path: 'showUnassigned',
              resolver: (value: any) => value = ''
            },
            {
              member: 'planState',
              operator: FilterOperator.IsEqualTo,
              path: 'showArchived',
              resolver: (value: any) => value = PlanStateEnum.Archived.toString()
            }
          ];
          filters = TdpUtils.resolveFilterDescriptors({
            filter: globalSearchFilter.selected.value,
            mappers
          });
          break;
        }
        case 'homeFilterProjectForm': {
          const mappers = [
            {
              member: 'projectName',
              operator: FilterOperator.Contains,
              path: 'projectName'
            },
            {
              member: 'projectReference',
              operator: FilterOperator.Contains,
              path: 'projectReference'
            },
            {
              member: 'builder.tradingName',
              operator: FilterOperator.Contains,
              path: 'builder.tradingName'
            },
            {
              member: 'createdDate',
              operator: FilterOperator.Contains,
              path: 'createdDate',
              resolver: (value: string) => {
                const [day, month, year] = value.split('/').map(Number);
                const isoString = new Date(Date.UTC(year, month - 1, day)).toISOString();
                return isoString.slice(0, -1); 
              }
            },
            {
              member: 'updatedDate',
              operator: FilterOperator.Contains,
              path: 'updatedDate',
              resolver: (value: string) => {
                const [day, month, year] = value.split('/').map(Number);
                const isoString = new Date(Date.UTC(year, month - 1, day)).toISOString();
                return isoString.slice(0, -1); 
              }
            },
           {
              member: 'isArchived',
              operator: FilterOperator.IsEqualTo,
              path: 'isArchived',
              resolver: (value: string) => (value ? 'True' : 'False')
            }
          ];
          filters = TdpUtils.resolveFilterDescriptors({
            filter: globalSearchFilter.selected.value,
            mappers
          });
          break;
        }
        case 'omniSearch':
          break;
        default:
          filters = [];
          break;
      }
      this.pageDescriptor.deleteAllFilters();
      this.pageDescriptor.addFilters(filters);
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'project.projectName',
      'project.projectReference',
      'project.lastUpdated',
      'notification.archiveProjectSuccess',
      'notification.archiveProjectError',
      'notification.unarchiveProjectSuccess',
      'notification.unarchiveProjectError',
      'notification.noProjectSearchResults',
      'notification.noActivePlans'
    ]).subscribe((translations) => {
      this.projectName = translations['project.projectName'];
      this.projectReference = translations['project.projectReference'];
      this.lastUpdated = translations['project.lastUpdated'];
      this.archiveProjectSuccess = translations['notification.archiveProjectSuccess'];
      this.archiveProjectError = translations['notification.archiveProjectError'];
      this.unarchiveProjectSuccess = translations['notification.unarchiveProjectSuccess'];
      this.unarchiveProjectError = translations['notification.unarchiveProjectError'];
      this.noProjectSearchResults = translations['notification.noProjectSearchResults'];
      this.noActivePlans = translations['notification.noActivePlans'];
    });
  }

  private loadData(): void {
    let subscription: Subscription;
    if (this.globalFilter && this.globalFilter.selected.key !== 'homeFilterProjectForm') {
      if (this.globalFilter.selected.key === 'omniSearch') {
        subscription = this.api.omniSearch.getOmniSearchResults({
          textToSearch: this.globalFilter.selected.value,
          pageSize: 0,
          pageNumber: 1
        }).subscribe((response) => {
           if (!response || response.omniSearchItemsList.length === 0) {
            this.notifications.success(this.noProjectSearchResults);
          }
          const projects: Project[] = response.omniSearchItemsList.filter(
            ({ type }) => type === 'ProjectModel'
          ).map(({ object }) => object) as Project[];
          this.tables = [
            this.buildProjects({ data: projects }, true),
          ];
        })
      }
    }else if (this.globalFilter && this.globalFilter.selected.key === 'homeFilterProjectForm') {
      if(!this.globalFilter.selected.value.isArchived) {
        const defaultFilter = new FilterDescriptor("isArchived", 2, "False");
        this.pageDescriptor.addOrUpdateFilter(defaultFilter);
      }
      subscription = this.api.projects.getProjectsFiltered(this.pageDescriptor)
      .subscribe((response) => {
        this.projects = response;
        this.tables = [this.buildProjects(response)];
        if (!response.data || response.data.length === 0) {
          this.notifications.success(this.noProjectSearchResults);
        }
      });
    }
    else {
      const defaultFilter = new FilterDescriptor("isArchived", 2, "False");
      this.pageDescriptor.addOrUpdateFilter(defaultFilter);
      subscription = this.api.projects.getProjectsFiltered(this.pageDescriptor)
      .subscribe((response) => {
        this.projects = response;
        this.tables = [this.buildProjects(response)];
        if (!response.data || response.data.length === 0) {
          this.notifications.success(this.noProjectSearchResults);
        }
      });
    }
    subscription && this.entitySubscriptions.push(subscription);
  }

  private buildProjects(records: TableRecords<Project>, isOmniSearch?: boolean): MultiTable<Project> {
    const datePipe = new DatePipe('en-US');
    return {
      key: "projects",
      columns: this.initializeProjectColumnConfigurationForBuilders(),
      label: "Recent Projects",
      pageChanged: (event) => {
        this.pageChanged(event);
      },
      tablePaginator: !isOmniSearch,
      tableSort: !isOmniSearch,
      pageSize: this.pageSize,
      records: {
        ...records,
        data: records.data.map(record => ({
          ...record,
          projectName: record.codeProject,
          projectReference: record.keyName,
          lastUpdated: datePipe.transform(record.updatedDate, 'dd/MM/yyyy')
        }))
      },
      rowMapper: (row) => row,
      rowClicked: (record) => {
        this.goToProjectDetails(record);
      },
      sortChanged: (event) => {
        this.sortChanged(event);
      },
    };
  }

  private initializeProjectColumnConfigurationForBuilders() {
    return (this.columnsConfiguration = [
      {
        columnDef: "projectName",
        header: this.projectName,
        sortField: "CodeProject",
        tooltipAtLength: 26,
        width: "20%",
      },
      {
        columnDef: "projectReference",
        header: this.projectReference,
        sortField: "KeyName",
        tooltipAtLength: 25,
        width: "20%",
      },
      {
        columnDef: "lastUpdated",
        header: this.lastUpdated,
        sortField: "UpdatedDate",
        tooltipAtLength: 26,
        width: "25%",
      },
    ]);
  }

  pageChanged(event: PageEvent) {
    this.pageDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.loadData();
  }

  private goToProjectDetails(project: Project) {
    this.navigateTo("/project/" + project.id); 
  }

  sortChanged(event: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(event);
    this.loadData();
  }

  public viewInCreatio(event: any, project: Project): void {
    event.stopPropagation();
    event.preventDefault();
    const projectIdGuid = project.keyName;
    const url = `https://${creatioEnvUrl}/0/Shell/#Card/aiepTenderProjects_FormPage/edit/${projectIdGuid}`;
    this.openExternalLink(url);
  }

  public async openExternalLink(url: string): Promise<void> {
    if (this.electronService.isElectronApp) {
      this.electronService.openExternalLink(url);
    } else {
      window.open(url, '_blank');
    }
  }

  public createPlan(event: any, project: Project): void {
    event.stopPropagation();
    event.preventDefault(); 
    this.dialogs.openCreateNewPlanModal({ data: null, project: project, projectWide: true});
  }

  public createTemplate(event: any, project: Project): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.openCreateNewTemplateModal({data:null, project: project, projectWide: true, navigation: true});
  }

  public openArchiveProjectDialog(event: MouseEvent, project: Project): void {
    this.dialogs.confirmation('dialog.archiveProject', 'dialog.archiveMessage')
    .then((confirmation) => {
      if (confirmation) {
        const projectStateSubscription = this.api.projects.changeProjectState(project.id, this.planStateEnum.Archived)
          .subscribe(
            (response) => {
              this.notifications.success(this.archiveProjectSuccess);
              setTimeout(() => {
                this.loadData();
              }, 2000);
            },
            (error) => {
              this.notifications.error(this.archiveProjectError);
            }
          );
        this.entitySubscriptions.push(projectStateSubscription);
      }
    });
  }

  public openRestoreDialog(event: MouseEvent, project: Project): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.confirmation('dialog.unarchiveProject', 'dialog.unarchiveProjectMessage')
    .then((confirmation) => {
      if (confirmation) {
        const projectStateSubscription =  this.api.projects.changeProjectState(project.id, this.planStateEnum.Active)
          .subscribe(
            (response) => {
              this.notifications.success(this.unarchiveProjectSuccess);
              setTimeout(() => {
                this.loadData();
              }, 2000);
            },
            (error) => {
              this.notifications.error(this.unarchiveProjectError);
            }
          );
        this.entitySubscriptions.push(projectStateSubscription);
      }
    });
  }

  public openTransferProjectDialog(event: MouseEvent, project: Project): void {
    event.stopPropagation();
    event.preventDefault();
  
    this.planService.getPlansForProject(project.id).subscribe((plans) => {
      const validPlans = (plans || []).filter(plan => !plan.isTemplate && plan.planState === 0);
  
      if (validPlans.length > 0) {
        this.dialogs.transferProject('70em', project, validPlans).then((result) => {
          if (result) {
            this.loadData();
          }
        });
      } else {
        this.notifications.warn(this.noActivePlans);
      }
    });
  }
}
