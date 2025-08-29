import { CdkScrollable, ScrollDispatcher } from '@angular/cdk/scrolling';
import {
  AfterViewInit,
  Component,
  ElementRef,
  NgZone,
  OnInit,
  TemplateRef,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { Subscription, zip } from 'rxjs';
import { CountryControllerBase } from 'src/app/core/services/country-controller/country-controller-base';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';
import { EnvelopeResponse } from 'src/app/core/services/url-parser/envelope-response.interface';
import { FilterDescriptor, FilterOperator } from 'src/app/core/services/url-parser/filter-descriptor.model';
import { TdpUtils } from 'src/app/core/utils';
import { ButtonFilterGroup } from 'src/app/shared/components/atoms/button-filter/button-filter.component';
import { ButtonFilter } from 'src/app/shared/components/atoms/button-filter/button-filter.types';
import { MultiTable } from 'src/app/shared/components/molecules/multi-table/types';
import { TableService } from 'src/app/shared/components/organisms/table/table.service';
import { TableColumnConfig, TableRecords } from 'src/app/shared/components/organisms/table/table.types';
import { Builder } from 'src/app/shared/models/builder';
import { GlobalSearchFilter } from 'src/app/shared/models/global-search-filter';
import { TdpPostCodePipe } from 'src/app/shared/pipes/pipes-postcode';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { environment } from '../../../environments/environment';
import { SortDescriptor, SortDirection } from '../..//core/services/url-parser/sort-descriptor.model';
import { ApiService } from '../../core/api/api.service';
import { ElectronService } from '../../core/electron-api/electron.service';
import { CommunicationService } from '../../core/services/communication/communication.service';
import { DialogsService } from '../../core/services/dialogs/dialogs.service';
import { PageDescriptor } from '../../core/services/url-parser/page-descriptor.model';
import { UserInfoService } from '../../core/services/user-info/user-info.service';
import { MiddlewareRomAndPreviewService } from '../../middleware/services/fusion-callbacks/rom-and-preview.service';
import { EducationToolMiddlewareService } from '../../middleware/services/Education-tool-middleware.service';
import { OfflineMiddlewareService } from '../../middleware/services/offline-middleware.service';
import { BaseComponent } from '../../shared/base-classes/base-component';
import { ComponentReloadData } from '../../shared/base-classes/reload-data-view';
import { PlanPreviewComponentData } from '../../shared/components/dialogs/plan-preview/plan-preview.model';
import { PlanPreviewService } from '../../shared/components/dialogs/plan-preview/plan-preview.service';
import {
  UploadedOfflinePlansDialogComponent
} from '../../shared/components/dialogs/uploaded-offline-plans-dialog/uploaded-offline-plans-dialog.component';
import { EducationToolType, PlanStateEnum } from '../../shared/models/app-enums';
import { Plan } from '../../shared/models/plan';
import { PlanOffline, SyncedPlan, VersionOffline } from '../../shared/models/plan-offline';
import { EducationToolService } from '../../core/Education-tool/Education-tool.service';

@Component({
  selector: 'tdp-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class HomeComponent extends BaseComponent implements OnInit, ComponentReloadData, AfterViewInit {

  public tables: MultiTable[];

  public plans: TableRecords<Plan>;
  public builders: TableRecords<Builder> = { data: [] };

  public minimizeHeader = false;

  public pageSize: number = 7;

  public columnsConfigurationForPlans: TableColumnConfig[];
  public columnsConfigurationForBuilders;
  public filters: ButtonFilter[];

  public offlinePlans: PlanOffline[] = [];
  public failToSyncPlans: PlanOffline[] = [];
  public failToSyncPlansIds: number[] = [];
  public syncedPlans: SyncedPlan[] = [];

  private pageDescriptor: PageDescriptor;
  private sortDescending: SortDescriptor;

  public releaseNotesRecoverErrorMsg = '';

  public globalFilter: GlobalSearchFilter = null;

  public showResetButton: boolean = true;
  public planStateEnum = PlanStateEnum;

  //Translation strings
  private unarchivePlanSuccess: string = '';
  private unarchivePlanError: string = '';
  private countryService: CountryControllerBase;
  selectedCountry: string = '';
  private planCode: string;
  private endUser: string;
  private tradingName: string;
  private Educationer: string;
  private version: string;
  private updatedDate: string;
  private accountNum: string;
  private address: string;
  private postcode: string;
  private recentPlans: string;
  private activePlans: string;
  private unassignedPlans: string;
  private tradeCustomers: string;
  private planTitle: string;
  private iconPersonal: string;
  private iconAiep: string;
  proToolEnabled: boolean;

  @ViewChild('header', { static: false }) header: ElementRef;

  @ViewChild(ButtonFilterGroup, { static: true }) filterGroup: ButtonFilterGroup;
  @ViewChild('openPreviewDialogActionRef', { static: false }) openPreviewDialogAction: TemplateRef<any>;

  constructor(
    private notifications: NotificationsService,
    private api: ApiService,
    private communications: CommunicationService,
    private dialogs: DialogsService,
    private dialogsPlanPreview: PlanPreviewService,
    private EducationToolService: EducationToolService,
    private electron: ElectronService,
    private middlewareService: EducationToolMiddlewareService,
    private userInfo: UserInfoService,
    private offlineMiddleware: OfflineMiddlewareService,
    private romAndPreviewMiddlewareService: MiddlewareRomAndPreviewService,
    private ngZone: NgZone,
    private matDialog: MatDialog,
    private tableService: TableService,
    private planDetailsService: PlanDetailsService,
    private scrollDispatcher: ScrollDispatcher,
    private zone: NgZone,
    private translate: TranslateService,
    private tdpPostCodePipe: TdpPostCodePipe,
    private country: CountryControllerService
  ) {
    super();
    this.plans = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.sortDescending = new SortDescriptor('UpdatedDate', SortDirection.Descending);
    this.countryService = this.country.getService();
  }

  get tableHeight(): string {
    if (this.globalFilter && this.globalFilter.selected.key === 'omniSearch') {
      return '700px';
    }
    else if (this.minimizeHeader) {
      return '650px';
    }
    else {
      return '600px';
    }
  }

  reloadDataView(): void {
    this.loadFilters();

    if (this.plans.data.length) {
      this.loadData();
    }
  }

  ngOnInit() {
    this.logger.info('Component initialization', 'HomeComponent');
    this.communications.notifyAiepSelectorEnabled(true);
    this.initializeTranslations();
    this.checkReleaseNotes();
    if (environment.appDynamics) {
      this.loadAppDynamics('adrum-4.5.4.1467.js');
    }
    if (this.electron.isElectronApp) {
      this.middlewareService.checkFusion();
      this.middlewareService.checkLicence();
      this.middlewareService.getFusionVersion();
      this.syncFromOffline();
    }
    this.filters = [
      { icon: 'fa-user', label: this.recentPlans, tag: this.iconPersonal, value: 'recent', checked: true },
      { icon: 'fa-user', label: this.activePlans, tag: this.iconPersonal, value: 'active' },
      { icon: 'fa-user', label: this.unassignedPlans, tag: this.iconPersonal, value: 'unassigned' },
      { icon: 'fa-store-alt', label: this.recentPlans, tag: this.iconAiep, value: 'recentAiep' },
      { icon: 'fa-store-alt', label: this.activePlans, tag: this.iconAiep, value: 'activeAiep' },
      { icon: 'fa-store-alt', label: this.unassignedPlans, tag: this.iconAiep, value: 'unassignedAiep' },
    ];
    this.initializeColumnConfigurationForPlans();
    this.initializeColumnConfigurationForBuilders();
    this.tables = [this.buildPlansTab({ data: [] })];
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.loadFilters();

    const globalSearchFilterSub = this.communications.subscribeToTopbarSearchChange().subscribe(globalFilter => {
      this.globalFilter = globalFilter;
      this.showResetButton = !globalFilter || globalFilter.selected.key !== 'omniSearch';
      if (this.globalFilter) {
        this.applyGlobalSearchFilters(globalFilter);
      } else {
        this.filterGroup.value && this.setFiltersByFilterValue(this.filterGroup.value);
      }
      this.resetView();
    });

    const filterGroupSubscription = this.filterGroup.valueChange.subscribe((value) => {
      if (!this.globalFilter) {
        const filter = this.setFiltersByFilterValue(value);
        filter && this.resetView();
      }
    });

    const reloadViewSubscription = this.communications.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });

    const topbarClearSubscription = this.communications.subscribeToTopbarClear(() => {
      if (this.filters && this.filters.length) {
        const [firstFilter] = this.filters;
        this.filterGroup.value = firstFilter.value;
      }
    });

    const AiepChangedSubscription = this.communications.subscribeToAiepChange(() => {
      this.loadFilters()
      this.resetView()
    })

    this.entitySubscriptions.push(
      filterGroupSubscription,
      reloadViewSubscription,
      globalSearchFilterSub,
      topbarClearSubscription,
      AiepChangedSubscription
    );
  }

  ngAfterViewInit() {
    this.scrollDispatcher.scrolled()
      .subscribe((cdk: CdkScrollable) => {
        this.zone.run(() => {
          const scrollPosition = cdk?.getElementRef().nativeElement.scrollTop;
          if (scrollPosition > this.header.nativeElement.offsetHeight) {
            this.minimizeHeader = true;
          }
        });
      });
  }

  public openCreatePlanModalHandler(event: MouseEvent, builder: Builder): void {
    event.stopPropagation();
    event.preventDefault();
    this.planDetailsService.setTradeCustomer(builder);
    this.dialogs.openCreatePlanModal()
  }

  private getEducationerFullName(record) {
    let name = record.Educationer?.firstName ? record.Educationer.firstName : '';
    let surname = record.Educationer?.surname ? record.Educationer.surname : '';
    let fullName = (name || surname) ? `${name} ${surname}` : '-';
    return fullName;
  }

  private buildPlansTab(records: TableRecords<Plan>, isOmniSearch?: boolean): MultiTable<Plan> {
    return {
      key: 'plans',
      columns: this.initializeColumnConfigurationForPlans(),
      label: this.planTitle,
      pageChanged: (event) => {
        this.pageChanged(event);
      },
      tablePaginator: !isOmniSearch,
      tableSort: !isOmniSearch,
      pageSize: this.pageSize,
      records,
      rowMapper: row => ({
        ...row,
        Educationer: {
          ...row.Educationer,
          fullName: this.getEducationerFullName(row)
        }
      }),
      rowClicked: (record) => {
        this.goToPlanDetails(record);
      },
      sortChanged: (event) => {
        this.sortChanged(event);
      }
    };
  }

  private buildTradeCustomersTab(records: TableRecords<Builder>, isOmniSearch?: boolean): MultiTable<Builder> {
    return {
      key: 'tradeCustomers',
      columns: this.initializeColumnConfigurationForBuilders(),
      label: this.tradeCustomers,
      pageChanged: (event) => {
        this.pageChanged(event);
      },
      tablePaginator: !isOmniSearch,
      tableSort: !isOmniSearch,
      pageSize: this.pageSize,
      records,
      rowMapper: row => ({
        ...row,
        postcode: this.tdpPostCodePipe.transform(row.postcode, true, this.countryService)
      }),
      rowClicked: (record) => {
        this.goToBuilderDetails(record);
      },
      sortChanged: (event) => {
        this.sortChanged(event);
      }
    };
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

  resetView() {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.tableService.resetCurrentTable();
    this.loadData();
  }

  resetTopbarForm(): void {
    this.communications.notifyClearTopbarValue();
  }

  pageChanged(event: PageEvent) {
    this.pageDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.loadData();
  }

  sortChanged(event: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(event);
    this.loadData();
  }

  private goToPlanDetails(plan: Plan) {
    this.navigateTo('/plan/' + plan.id);
  }

  private goToBuilderDetails(builder: Builder): void {
    this.navigateTo(`/builder/${builder.id}`);
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
    if (plan.masterVersionId) {
      this.goToPlanDetails(plan);
      this.EducationToolService.getPlanDetailsAndOpenInEducationTool({
        planId: plan.id,
        builderId: plan.builderId,
        catalogId: plan.catalogId,
        versionId: plan.masterVersionId,
        EducationOrigin: plan.EducationOrigin
      });
    } else {
      this.dialogs.simpleInformation('dialog.emptyPlanTitle', 'dialog.emptyPlanMsg');
    }

  }

  openPlanTest() {
    // @ts-ignore
    this.EducationToolService.createNewPlan({
      // @ts-ignore
      planId: 2,
      // @ts-ignore
      builderId: 2,
      catalogId: 3,
      versionId: 5,
    }, EducationToolType.THREE_DC);
  }

  public restoreArchivedPlan(event: MouseEvent, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.confirmation('dialog.unarchivePlan', 'dialog.unarchiveMessage')
      .then((confirmation) => {
        if (confirmation) {
          const planStateSubscription = this.api.plans.changePlanState(plan.id, this.planStateEnum.Active)
            .subscribe(
              (response) => {
                this.notifications.success(this.unarchivePlanSuccess);
                plan.planState = response.planState;
                this.loadData();
              },
              (error) => {
                this.notifications.error(this.unarchivePlanError);
              }
            );
          this.entitySubscriptions.push(planStateSubscription);
        }
      });
  }

  public openTransferSinglePlanDialog(event: any, plan: Plan): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.transferSinglePlan('70em', plan);
  }

  public openTransferBuilderPlansDialog(event: MouseEvent, builder: Builder): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogs.transferBuilderPlans('70em', builder.id);
  }

  public openMainPlanDetails(): void {
    this.planDetailsService.openMainPlanDetails(this.countryService);
  }

  private getRecentPlansNumber(personal = false) {
    this.pageDescriptor.deleteAllFilters();

    if (personal) {
      const personalFilter = new FilterDescriptor('EducationerId', FilterOperator.IsEqualTo, this.userInfo.getId());
      this.pageDescriptor.addOrUpdateFilter(personalFilter);
    }

    const recentDate = new Date();
    recentDate.setDate(recentDate.getDate() - 28);
    const recentFilter = new FilterDescriptor('updatedDate', FilterOperator.IsGreaterThanOrEqualTo, recentDate);
    this.pageDescriptor.addOrUpdateFilter(recentFilter);

    this.pageDescriptor.setPagination(0, 0);
    const pageDescriptor: any = { ...this.pageDescriptor };
    return this.api.plans.getPlansFiltered(pageDescriptor);
  }

  private getActivePlansNumber(personal = false) {
    this.pageDescriptor.deleteAllFilters();

    if (personal) {
      const personalFilter = new FilterDescriptor('EducationerId', FilterOperator.IsEqualTo, this.userInfo.getId());
      this.pageDescriptor.addOrUpdateFilter(personalFilter);
    }

    this.pageDescriptor.setPagination(0, 0);
    const pageDescriptor: any = { ...this.pageDescriptor };
    return this.api.plans.getPlansFiltered(pageDescriptor);
  }

  private getUnassignedPlansNumber(personal = false) {
    this.pageDescriptor.deleteAllFilters();

    if (personal) {
      const personalFilter = new FilterDescriptor('EducationerId', FilterOperator.IsEqualTo, this.userInfo.getId());
      this.pageDescriptor.addOrUpdateFilter(personalFilter);
    }

    const unassignedFilter = new FilterDescriptor('builderId', FilterOperator.IsEqualTo, '');
    this.pageDescriptor.addOrUpdateFilter(unassignedFilter);

    this.pageDescriptor.setPagination(0, 0);
    const pageDescriptor: any = { ...this.pageDescriptor };
    return this.api.plans.getPlansFiltered(pageDescriptor);
  }

  private updateKpiFilters(filters: EnvelopeResponse<Plan>[]): void {
    this.filters.forEach((filter, index) => filter.number = filters[index].total);
    this.pageDescriptor.setPagination(0, this.pageSize);
  }

  private loadFilters() {
    const filtersSubscription =
      zip(
        this.getRecentPlansNumber(true),
        this.getActivePlansNumber(true),
        this.getUnassignedPlansNumber(true),
        this.getRecentPlansNumber(),
        this.getActivePlansNumber(),
        this.getUnassignedPlansNumber(),
      ).pipe()
        .subscribe((filterResult) => {
          this.updateKpiFilters(filterResult);
        });

    this.entitySubscriptions.push(filtersSubscription);
  }

  private loadData() {
    let subscription: Subscription;
    if (this.globalFilter && this.globalFilter.selected.key !== 'homeFilterPlanForm') {
      if (this.globalFilter.selected.key === 'homeFilterBuilderForm') {
        subscription = this.api.builders.getBuildersFiltered(
          this.pageDescriptor
        ).subscribe((response) => {
          this.builders = response;
          this.plans = { data: [] };
          this.tables = [this.buildTradeCustomersTab(response)];
          this.savePageDescriptorToLocalStorage(this.pageDescriptor);
        });
      } else if (this.globalFilter.selected.key === 'omniSearch') {
        subscription = this.api.omniSearch.getOmniSearchResults({
          textToSearch: this.globalFilter.selected.value,
          pageSize: 0,
          pageNumber: 1
        }).subscribe((response) => {
          const plans: Plan[] = response.omniSearchItemsList.filter(
            ({ type }) => type === 'PlanModel'
          ).map(({ object }) => object) as Plan[];
          const builders: Builder[] = response.omniSearchItemsList.filter(
            ({ type }) => type === 'BuilderModel'
          ).map(({ object }) => object) as Builder[];
          this.tables = [
            this.buildPlansTab({ data: plans }, true),
            this.buildTradeCustomersTab({ data: builders }, true)
          ];
        })
      }
    } else {
      subscription = this.api.plans.getPlansFiltered(
        this.pageDescriptor
      ).subscribe((response) => {
        this.plans = response;
        this.builders = { data: [] };
        this.tables = [this.buildPlansTab(response)];
        this.savePageDescriptorToLocalStorage(this.pageDescriptor);
      });
    }
    subscription && this.entitySubscriptions.push(subscription);
  }

  private savePageDescriptorToLocalStorage(descriptor: PageDescriptor): void {
    localStorage.setItem('pageDescriptor', JSON.stringify(descriptor));
  }

  private setFiltersByFilterValue(value: string) {
    if (!value) {
      return;
    }

    this.pageDescriptor.deleteAllFilters();

    if (value === 'recent' || value === 'active' || value === 'unassigned') {
      const personalFilter = new FilterDescriptor('EducationerId', FilterOperator.IsEqualTo, this.userInfo.getId());
      this.pageDescriptor.addOrUpdateFilter(personalFilter);
    }

    switch (value) {
      case 'recent':
      case 'recentAiep':
        const recentDate = new Date();
        recentDate.setDate(recentDate.getDate() - 28);
        const recentFilter = new FilterDescriptor('updatedDate', FilterOperator.IsGreaterThanOrEqualTo, recentDate);
        this.pageDescriptor.addOrUpdateFilter(recentFilter);
        return true;

      case 'active':
      case 'activeAiep':
        return true;

      case 'unassigned':
      case 'unassignedAiep':
        const unassignedFilter = new FilterDescriptor('builderId', FilterOperator.IsEqualTo, '');
        this.pageDescriptor.addOrUpdateFilter(unassignedFilter);
        return true;

      default:
        break;
    }
  }

  private loadAppDynamics(scriptName: string): void {
    const script = document.createElement('script');
    script.src = scriptName;
    document.body.appendChild(script);
  }

  private checkReleaseNotes(): void {
    const subscription = this.userInfo.getUserInfo$().subscribe(userInfo => {
      const { showReleaseInfoId } = userInfo;
      this.proToolEnabled = userInfo.proToolEnabled;
      if (showReleaseInfoId) {
        const releaseInfoSubscription = this.api.releaseInfo.getReleaseInfoDocument(showReleaseInfoId)
          .subscribe((releaseNotesDocument) => {
            this.dialogs.pdfPreview(releaseNotesDocument, 'release_notes');
            const markReadSubscribe = this.api.releaseInfo.markReleaseInfoAsRead(showReleaseInfoId)
              .subscribe();
            this.entitySubscriptions.push(markReadSubscribe);
          }, () => {
            this.notifications.error(this.releaseNotesRecoverErrorMsg);
          });
        this.entitySubscriptions.push(releaseInfoSubscription);
      }
    });
    this.entitySubscriptions.push(subscription);
  }

  private syncFromOffline() {
    if (localStorage.getItem('plansSynced') !== 'true') {
      this.readOfflinePlansJSON();
    }
  }

  private readOfflinePlansJSON(): void {
    const readPlansSubscription = this.offlineMiddleware.readPlansObservable()
      .subscribe((fileList) => {
        if (fileList !== null && fileList !== '') {
          const data = {
            title: 'dialog.plansToBeSynced.title',
            description: 'dialog.plansToBeSynced.message1',
            button: 'button.accept',
            description2: 'dialog.plansToBeSynced.message2'
          }
          this.dialogs.offlineSimpleDialog(
            data
          ).then(() => {
            localStorage.setItem('plansSynced', 'true');
            this.offlinePlans = fileList.obj;
            this.syncOfflinePlans(0);
          });
        }
      });
    this.entitySubscriptions.push(readPlansSubscription);
  }

  public syncOfflinePlans(pos: number): void {
    if (pos < this.offlinePlans.length) {
      const planToCreate: Plan = {
        id: 0,
        cadFilePlanId: '',
        title: '',
        lastOpen: new Date(),
        updatedDate: new Date(),
        createdDate: new Date(),
        catalogId: this.offlinePlans[pos].catalogueId,
        EducationerId: 0,
        Educationer: null,
        projectId: 0,
        project: null,
        versions: [],
        keyName: '',
        survey: this.offlinePlans[pos].survey,
        planCode: '',
        endUser: null,
        endUserId: null,
        endUserFullName: '',
        planState: 0,
        builderId: null,
        masterVersionId: null,
        builderTradingName: '',
        planType: '',
        notes: '',
        masterVersion: null,
        planName: this.offlinePlans[pos].planName,
        offlineSyncDate: new Date(),
        isStarred: false
      };

      const planCreationSubscription = this.api.plans.createSinglePlan(planToCreate)
        .subscribe(
          (response) => {
            this.syncedPlans.push({ idOffline: this.offlinePlans[pos].id_offline, planCode: response.planCode });
            this.syncOfflinePlansVersion(this.offlinePlans[pos].planNumber, response.id, pos, 0);
          }, (error) => {
            this.failToSyncPlans.push(this.offlinePlans[pos]);
            this.failToSyncPlansIds.push(this.offlinePlans[pos].id_offline);
            this.syncOfflinePlans(pos + 1);
          });
      this.entitySubscriptions.push(planCreationSubscription);
    } else {
      if (this.offlinePlans.length > 0) {
        this.ngZone.run(() => {
          const dialogRef = this.matDialog.open(UploadedOfflinePlansDialogComponent, {
            width: '800px',
            data: { syncedPlans: this.syncedPlans, notSyncedPlans: this.failToSyncPlansIds }
          });
          dialogRef.afterClosed().subscribe((data) => {
            if (data) {
              this.filterGroup._selectValue('unassignedAiep');
            }
          });
        });
        if (this.failToSyncPlans.length > 0) {
          this.offlineMiddleware.writePlans(this.failToSyncPlans);
        } else {
          this.offlineMiddleware.deleteFile();
        }
      }
    }
  }

  public syncOfflinePlansVersion(planNumber: string, syncedPlanId: number, posPlan: number, posVersion: number): void {
    if (posVersion < this.offlinePlans[posPlan].versions.length) {
      const getRomAndPreview = this.romAndPreviewMiddlewareService.subscribeToRomAndPreviewFile((data) => {
        if (data && posPlan === data.posPlan && posVersion === data.posVersion) {
          const romFile = data.rom;
          const previewFile = data.preview;
          const subscription = this.api.plans.saveVersion(
            syncedPlanId,
            this.generateFormData(this.offlinePlans[posPlan].versions[posVersion], syncedPlanId, romFile, previewFile, posPlan)
          ).subscribe((response) => {
            this.syncOfflinePlansVersion(planNumber, syncedPlanId, posPlan, posVersion + 1);
          }, (error) => {
            this.syncedPlans.pop();
            this.failToSyncPlans.push(this.offlinePlans[posPlan]);
            this.failToSyncPlansIds.push(this.offlinePlans[posPlan].id_offline);
            this.syncOfflinePlans(posPlan + 1);
          });
          this.entitySubscriptions.push(subscription);
        }
      });
      this.entitySubscriptions.push(getRomAndPreview);
      const model = {
        romFilePath: this.offlinePlans[posPlan].versions[posVersion].romPath,
        previewFilePath: this.offlinePlans[posPlan].versions[posVersion].previewPath,
        posPlan: posPlan,
        posVersion: posVersion
      };
      this.middlewareService.getRomAndPreview(model);
    } else {
      this.syncOfflinePlans(posPlan + 1);
    }
  }

  private generateFormData(version: VersionOffline, syncedPlansId: number, romFile: any, previewFile: any, posPlan: any): FormData {
    const romfileName = syncedPlansId + '_' + 'Rom' + '.Rom';
    const previewfileName = syncedPlansId + '_' + 'Preview' + '.jpeg';

    const rom = new File(
      [romFile as ArrayBuffer],
      romfileName,
      { type: 'application/octet-stream' }
    );

    const preview = new File(
      [previewFile as ArrayBuffer],
      previewfileName,
      { type: 'image/jpeg' }
    );

    const formData = new FormData();
    formData.append('romFile', rom, romfileName);
    formData.append('previewFile', preview, previewfileName);
    formData.append('model', JSON.stringify({
      id: 0,
      catalogCode: version.catalogueCode,
      versionNotes: this.offlinePlans[posPlan].EducationerName ?
        'Offline Educationer: ' + this.offlinePlans[posPlan].EducationerName +
        '.\n' + version.versionNotes : version.versionNotes,
      quoteOrderNumber: version.quoteOrderNumber,
      range: version.range,
      romItems: version.romItems,
      isSync: true
    }));
    return formData;
  }

  private initializeTranslations() {
    const translationsSubscription = this.translate.get([
        'notification.unarchivePlanSuccess',
        'notification.unarchivePlanError',
        'plan.planId',
        'plan.endUser',
        'builder.tradingName',
        'plan.Educationer',
        'plan.versionId',
        'plan.lastUpdated',
        'builder.accountNumber',
        'builder.address',
        'builder.postcode',
        'button.recentPlans',
        'builder.activePlans',
        'filters.unassignedPlans',
        'home.tradeCustomers',
        'home.planTitle',
        'home.personal',
        'login.Aiep',
        'topbar.releaseNotesRecoveringError'
      ],
    ).subscribe((translations) => {
      this.unarchivePlanSuccess = translations['notification.unarchivePlanSuccess'];
      this.unarchivePlanError = translations['notification.unarchivePlanError'];
      this.planCode = translations['plan.planId'];
      this.endUser = translations['plan.endUser'];
      this.tradingName = translations['builder.tradingName'];
      this.Educationer = translations['plan.Educationer'];
      this.version = translations['plan.versionId'];
      this.updatedDate = translations['plan.lastUpdated'];
      this.accountNum = translations['builder.accountNumber'];
      this.address = translations['builder.address'];
      this.postcode = translations['builder.postcode'];
      this.recentPlans = translations['button.recentPlans'];
      this.activePlans = translations['builder.activePlans'];
      this.unassignedPlans = translations['filters.unassignedPlans'];
      this.tradeCustomers = translations['home.tradeCustomers'];
      this.planTitle = translations['home.planTitle'];
      this.iconPersonal = translations['home.personal'];
      this.iconAiep = translations['login.Aiep'];
      this.releaseNotesRecoverErrorMsg = translations['topbar.releaseNotesRecoveringError'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }

  private initializeColumnConfigurationForPlans() {
    return this.columnsConfigurationForPlans = [
      { columnDef: 'planCode', header: this.planCode, sortField: 'PlanCode', tooltipAtLength: 23 },
      { columnDef: 'endUser.fullName', header: this.endUser, field: 'endUser.fullName', sortField: 'EndUser.Surname', tooltipAtLength: 23 },
      { columnDef: 'builderTradingName', header: this.tradingName, sortField: 'BuilderTradingName', tooltipAtLength: 23 },
      { columnDef: 'Educationer.fullName', header: this.Educationer, sortField: 'Educationer.Surname', tooltipAtLength: 23 },
      { columnDef: 'updatedDate', header: this.updatedDate, sortField: 'UpdatedDate', tooltipAtLength: 23, isDate: true }
    ];
  }

  private initializeColumnConfigurationForBuilders() {
    return this.columnsConfigurationForBuilders = [
      { columnDef: 'accountNumber', header: this.accountNum, sortField: 'AccountNumber', tooltipAtLength: 26 },
      { columnDef: 'tradingName', header: this.tradingName, sortField: 'TradingName', tooltipAtLength: 25 },
      { columnDef: 'address0', header: this.address, field: 'address0', sortField: 'Address1', tooltipAtLength: 26 },
      { columnDef: 'postcode', header: this.postcode, sortField: 'Postcode', tooltipAtLength: 26 }
    ];
  }
}


