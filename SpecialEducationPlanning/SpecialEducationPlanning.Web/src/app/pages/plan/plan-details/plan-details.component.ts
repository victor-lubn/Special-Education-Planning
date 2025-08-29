import { Component, NgZone, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup, Validators } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { zip } from 'rxjs';
import { finalize, map } from 'rxjs/operators';
import { CountryControllerBase } from 'src/app/core/services/country-controller/country-controller-base';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';
import { UserEducationer } from 'src/app/shared/models/Educationer.model';
import { RenderingStatus, SuccessStatus } from 'src/app/shared/models/publish-status-const';
import { PublishJob } from 'src/app/shared/models/publish-job.model';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { ApiService } from '../../../core/api/api.service';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { DownloadFileService } from '../../../core/services/download-file/download-file.service';
import { PageDescriptor } from '../../../core/services/url-parser/page-descriptor.model';
import { UserInfoService } from '../../../core/services/user-info/user-info.service';
import {
  MiddlewareRecoverPlanAutosaveService
} from '../../../middleware/services/fusion-callbacks/recover-plan-autosave.service';
import { MiddlewareSavedPlanService } from '../../../middleware/services/fusion-callbacks/saved-plan.service';
import { EducationToolMiddlewareService } from '../../../middleware/services/Education-tool-middleware.service';
import { FormComponent } from '../../../shared/base-classes/form-component';
import { ComponentReloadData } from '../../../shared/base-classes/reload-data-view';
import { PlanPreviewComponentData } from '../../../shared/components/dialogs/plan-preview/plan-preview.model';
import { PlanPreviewService } from '../../../shared/components/dialogs/plan-preview/plan-preview.service';
import {
  UnableAutosaveRecoverDialogComponent
} from '../../../shared/components/dialogs/unable-autosave-recover/unable-autosave-recover.component';
import { EndUserFormComponent } from '../../../shared/components/forms/end-user-form/end-user-form.component';
import { TableColumnConfig, TableRecords } from '../../../shared/components/organisms/table/table.types';
import { TimelineService } from '../../../shared/components/organisms/timeline/timeline.service';
import { SidebarService } from '../../../shared/components/sidebar/sidebar.service';
import { ComponentCanDeactivate } from '../../../shared/guards/pending-changes.guard';
import {
  ActionTypeEnum,
  EducationToolType,
  PlanStateEnum,
  PublishAssetTypeEnum,
  PublishStatusEnum,
  TimelineItemTypeEnum
} from '../../../shared/models/app-enums';
import { Builder } from '../../../shared/models/builder';
import { Catalog } from '../../../shared/models/catalog.model';
import { EndUser } from '../../../shared/models/end-user';
import { Plan } from '../../../shared/models/plan';
import { TimelineItem } from '../../../shared/models/timeline-item';
import { Version } from '../../../shared/models/version';
import { PublishAsset } from 'src/app/shared/models/publish-asset.model';
import { TenderPackPlanPayload } from 'src/app/shared/models/tenderPack-plan-payload';
import { EducationToolService } from '../../../core/Education-tool/Education-tool.service';

@Component({
  selector: 'tdp-plan-details',
  templateUrl: './plan-details.component.html',
  styleUrls: ['./plan-details.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PlanDetailsComponent extends FormComponent implements OnInit, ComponentCanDeactivate, ComponentReloadData {
  public sidebar: string = 'timelineSidebar'
  public endUserValidation: any;
  public originalPlan: Plan;
  public originalBuilder: Builder;
  public originalEndUser: EndUser;
  public endUser: EndUser;
  public timelineItems: TimelineItem[] = [];
  public timelineComments: TimelineItem[] = [];
  public planDetails: UntypedFormGroup;
  public planVersions: Version[] = [];
  public actionTypeEnum = ActionTypeEnum;
  public timelineTypeEnum = TimelineItemTypeEnum;
  public catalogs: Catalog[];
  public planStateEnum = PlanStateEnum;
  public possibleBuilders: Builder[];
  public selectedBuilderId: number;
  public isEditing: boolean;
  public isAssigned: boolean;
  public originalEducationer: UserEducationer
  public currentValue: string;
  public loadingImage: boolean;
  public noImageAvailable: boolean;
  public previewUnavailable: boolean;
  public imageUrl: SafeUrl;
  public masterVersionIndex: number;
  public showRestoreButton: boolean = false;

  public versionsTable: TableRecords<Version>;
  protected versionsPageDescriptor: PageDescriptor;
  public pageSize: number = 14;
  public currentPlanMasterVersion: Version;

  public publishJobs: PublishJob[] = [];
  public publishJobsTable: TableRecords<PublishJob>;
  public masterPublishStatus: PublishStatusEnum;
  public publishJobMasterVersion: PublishJob;
  private imageBlob: Blob;
  private imageBuffer: ArrayBuffer;
  //Translation strings
  protected currentPlanId: number;
  protected markMasterVersionError: string =  '';
  protected archivePlanSuccess: string = '';
  protected archivePlanError: string = '';
  protected unarchivePlanSuccess: string = '';
  protected unarchivePlanError: string = '';
  protected planChangesSaved: string = '';
  protected planUpdatedSuccess: string = '';
  protected planSaveError: string = '';
  protected planUpdatedError: string = '';
  protected saveAutosaveSuccess: string = '';
  protected saveAutosaveError: string = '';
  protected unableToRecoverAutosaveTitle: string = '';
  protected unableToRecoverAutosaveMessage: string = '';
  protected recoverLastAutosaveTitle: string = '';
  protected recoverLastAutosaveMessage: string = '';
  protected versionNumberHeader: string = '';
  protected versionIdHeader: string = '';
  protected rangeHeader: string = '';
  protected versionNotesHeader: string = '';
  protected orderNumberHeader: string = '';
  protected lastUpdatedHeader: string = '';
  protected renderTypeHeader: string = '';
  protected dateHeader: string = '';
  protected stateHeader: string = '';
  protected versionPublishingTitle: string = '';
  protected versionPublishingMessage: string = '';
  protected versionNotPublishedTitle: string = '';
  protected versionNotPublishedMessage: string = '';
  protected versionNotPublishedMessage2: string = '';
  protected renderingMsg: string = '';
  protected emailSentMsg: string = '';

  protected countryService: CountryControllerBase;
  public versionsColumnNames: TableColumnConfig[] = [];
  public publishJobsColumnNames: TableColumnConfig[] = [];
  public isTenderPack: boolean = false;

  @ViewChild('catalogueTrigger', { read: MatAutocompleteTrigger, static: true }) catalogueTrigger: MatAutocompleteTrigger;
  @ViewChild('planTypeTrigger', { read: MatAutocompleteTrigger, static: true }) planTypeTrigger: MatAutocompleteTrigger;

  @ViewChild(EndUserFormComponent)
  private endUserForm: EndUserFormComponent;

  EducationToolType = EducationToolType;

  constructor(
    protected activatedRoute: ActivatedRoute,
    protected api: ApiService,
    protected dialogs: DialogsService,
    protected notifications: NotificationsService,
    protected downloadService: DownloadFileService,
    protected translate: TranslateService,
    protected communication: CommunicationService,
    protected recoverPlanAutosaveService: MiddlewareRecoverPlanAutosaveService,
    protected matDialog: MatDialog,
    protected ngZone: NgZone,
    protected middlewareService: EducationToolMiddlewareService,
    protected savedPlanMiddlewareService: MiddlewareSavedPlanService,
    protected userInfo: UserInfoService,
    protected sanitizer: DomSanitizer,
    protected EducationToolService: EducationToolService,
    protected sidebarService: SidebarService,
    protected timelineService: TimelineService,
    protected planDetailsService: PlanDetailsService,
    protected dialogsPlanPreview: PlanPreviewService,
    protected countryControllerSvc: CountryControllerService
  ) {
    super();
    this.countryService = this.countryControllerSvc.getService();
    this.possibleBuilders = [];
    this.planVersions = [];
    this.planDetails = this.formBuilder.group({
      planCode: [null],
      planName: [null],
      builderTradingName: [null],
      catalogId: [null, Validators.required],
      survey: [false],
      hasEndUser: [false],
      masterVersionId: [null],
      cadFilePlanId: [null],
      planType: [null],
      EducationerId: [null]
    });
    this.planDetails.disable();
    this.isEditing = false;
    this.isAssigned = true;
    this.loadingImage = true;
    this.noImageAvailable = false;
    this.previewUnavailable = false;;
    this.imageBuffer = new ArrayBuffer(0);
  }

  ngOnInit(): void {
    this.versionsTable = { data: [] };
    this.publishJobsTable = { data: [] };
    this.versionsPageDescriptor = new PageDescriptor();
    this.versionsPageDescriptor.setPagination(0, this.pageSize);
    this.communication.notifyClearHomeFilters(false);
    this.communication.notifyAiepSelectorEnabled(false);
    this.activatedRoute.queryParams.subscribe((params) => {
      this.isTenderPack = params['isTenderPack'];
    });
    const routerSubscription = this.activatedRoute.params.subscribe((params) => {
      this.currentPlanId = +params['id'];
      this.recoverViewData(this.currentPlanId);
      this.initializeReloadDataView();
    });
    this.entitySubscriptions.push(routerSubscription);

    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
  }

  public loadPlanVersions(versions: Version[]): void {
    this.versionsTable = { data: versions, total: versions.length }
  }

  public loadPlanPublishJobs(publishJobs: PublishJob[]): void {
    const data = this.sortPublishJobsDesc(publishJobs);
    this.publishJobsTable = {data: data, total: publishJobs.length}
  }

  public getPlanVersions(planId: number) {
    return this.api.plans.getPlanVersions(planId);
  }

  getEndUserValidation(): any {
    let output = this.checkEndUserValidation();
    return output;
  }

  checkEndUserValidation(): any {
    return {
      titleId: [null],
      firstName: [null],
      surname: [null, Validators.required],
      contactEmail: [null],
      countryCode: [null],
      country: [null],
      postcode: [null, [Validators.required]],
      address1: [null, Validators.required],
      address2: [null],
      address3: [null],
      address4: [null],
      address5: [null],
      mobileNumber: [null],
      landLineNumber: [null],
      comment: [null]
    };
  }

  private initializeReloadDataView() {
    const reloadViewDataSubscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(reloadViewDataSubscription);
  }

  hasChanges(): boolean {
    return this.planDetails.dirty || (this.endUserForm && this.endUserForm.entityForm.dirty);
  }

  reloadDataView(): void {
    this.recoverViewData(this.currentPlanId);
  }

  public goToBuilder(): void {
    this.navigateTo('/builder/' + this.originalPlan.builderId);
  }

  public markMasterVersionHandler(event: any, versionId: number): void {
    event.stopPropagation();
    event.preventDefault();
    if (this.originalPlan.masterVersionId !== versionId) {

      const newMasterVersion = this.getMasterVersion(versionId);
      this.originalPlan.masterVersionId = versionId;
      this.originalPlan.catalogId = newMasterVersion.catalogId;

      const subscription = this.api.plans.updatePlan(this.originalPlan)
        .subscribe(
          (response) => {
            const newMasterVersion = this.getMasterVersion(versionId);
            const restOfVersions = this.planVersions.filter(version => version.id !== versionId);
            this.planVersions = [newMasterVersion, ...restOfVersions];
            this.reloadDataView();
          },
          (error) => {
            this.notifications.error(this.markMasterVersionError);
          }
        );
      this.entitySubscriptions.push(subscription);
    }
  }

  public getMasterVersion(masterVersionId: number): Version {
    return this.planVersions.find(version => version.id === masterVersionId);
  }

  public getMasterVersionIndex(masterVersionId: number): number {
    return this.masterVersionIndex = this.planVersions.findIndex(version => version.id === masterVersionId);
  }

  checkVersionStatus(record: any) {
    return record === this.getMasterVersion(this.originalPlan.masterVersionId) ? true : false;
  }

  public unassignPlan(): void {
    this.dialogs.unassignTradeCustomer(
      this.originalBuilder.tradingName,
    ).then((confirmation) => {
      if (confirmation) {
        this.dialogs.simpleInformation(
          'dialog.unassignBuilder.stepsTitle',
          'dialog.unassignBuilder.stepsMessage',
          'dialog.unassignBuilder.stepsMessage2',
        ).then((confirm) => {
          this.api.plans.unassignBuilderFromPlan(this.currentPlanId)
            .subscribe((response => {
              this.notifications.success(this.planChangesSaved, this.planUpdatedSuccess);
              this.reloadDataView();
            }), error => {
              this.notifications.error(this.planSaveError, this.planUpdatedError);
            });
        });
      }
    });
  }

  public submitPlanUpdate(planObject: Plan): void {
    if (this.planDetails.valid) {
      const updatePlanSubscription = this.api.plans.updatePlan({
        ...this.originalPlan,
        ...planObject
      }).subscribe((planResponse: Plan) => {
        this.notifications.success(this.planChangesSaved, this.planUpdatedSuccess);
        // this.originalPlan = planResponse;
        this.isEditing = false;
        // this.planDetails.patchValue(planResponse);
        // this.planDetails.get('hasEndUser').patchValue(!!planResponse.endUserId);
        this.planDetails.disable();
        this.planDetails.markAsPristine();
        // if (planResponse.endUserId) {
        //   this.endUser = planResponse.endUser;
        //   this.originalEndUser = this.endUser;
        //   this.endUserForm.cancelEdit(this.originalEndUser);
        // } else {
        //   this.endUser = null;
        //   this.originalEndUser = null;
        // }
        this.reloadDataView();
      }, (error) => {
        this.notifications.error('notification.saveError', 'notification.planUpdatedError');
      });
      this.entitySubscriptions.push(updatePlanSubscription);
    }
  }

  public onUpdatePlan(planFormGroup): void {
    if (!planFormGroup.get('hasEndUser').value && this.originalPlan.endUserId) {
      this.dialogs.confirmation('dialog.removeEndUserTitle', 'dialog.removeEndUserMsg').then((confirmation) => {
        if (confirmation) {
          const planDetails = {
            ...planFormGroup.value,
            endUser: null,
            endUserId: null
          }
          this.updatePlan(planDetails);
        }
      });
    } else {
      this.updatePlan(planFormGroup.value);
    }
  }

  public onUpdateTenderPackPlan(planDetails: TenderPackPlanPayload) {
    const updateSubscription = this.api.plans.updateTenderPackPlan(planDetails).subscribe((planResponse: Plan) => {
      this.notifications.success(this.planChangesSaved, this.planUpdatedSuccess);
      this.reloadDataView();
    }, (error) => {
      this.notifications.error('notification.saveError', 'notification.planUpdatedError');
    });
    this.entitySubscriptions.push(updateSubscription);
  }

  public onUpdateEndUser(endUser: UntypedFormGroup): void {
    const endUserDetails = {
      ...this.originalEndUser,
      ...endUser.value
    }
    this.validateExistingEndUser(endUserDetails);
  }

  public downloadPlanRom(versionId: number): void {
    const subscription = this.api.plans.getVersionFile(versionId)
      .subscribe((response) => {
        this.downloadService.downloadFile(
          new Blob([response.romByteArray], { type: response.type }),
          response.fileName
        );
      });
    this.entitySubscriptions.push(subscription);
  }

  public uploadVersionRom(versionId: number): void {
    this.dialogs.uploadPlan('40em', versionId, this.originalPlan.id);
  }

  public editVersionNotes(versionId: number): void {
    this.dialogs.editVersionNotes(versionId);
  }

  public uploadPlan(): void {
    // Version ID must be set to 0 to create a new Version
    this.dialogs.uploadPlan('40em', 0, this.originalPlan.id);
  }

  public archivePlan(): void {
    this.dialogs.confirmation('dialog.archivePlan', 'dialog.archiveMessage')
      .then((confirmation) => {
        if (confirmation) {
          const planStateSubscription = this.api.plans.changePlanState(this.originalPlan.id, this.planStateEnum.Archived)
            .subscribe(
              (response) => {
                this.notifications.success(this.archivePlanSuccess);
                this.originalPlan.planState = response.planState;
              },
              (error) => {
                this.notifications.error(this.archivePlanError);
              }
            );
          this.entitySubscriptions.push(planStateSubscription);
        }
      });
  }

  public restoreArchivedPlan(): void {
    this.dialogs.confirmation('dialog.unarchivePlan', 'dialog.unarchiveMessage')
      .then((confirmation) => {
        if (confirmation) {
          const planStateSubscription = this.api.plans.changePlanState(this.originalPlan.id, this.planStateEnum.Active)
            .subscribe(
              (response) => {
                this.notifications.success(this.unarchivePlanSuccess);
                this.originalPlan.planState = response.planState;
              },
              (error) => {
                this.notifications.error(this.unarchivePlanError);
              }
            );
          this.entitySubscriptions.push(planStateSubscription);
        }
      });
  }

  saveLastPlanAutosaveThreeDC() {
    this.api.plans.getPlan(this.currentPlanId).subscribe((plan) => {
      const masterVersion = plan.versions.find(v => v.id === plan.masterVersionId);
      if (!masterVersion || masterVersion.lastKnown3DCVersion > masterVersion.EducationTool3DCVersionId) {
        this.ngZone.run(() => {
          this.dialogs.confirmation(this.recoverLastAutosaveTitle, this.recoverLastAutosaveMessage + plan.planCode + '?')
            .then((confirmation) => {
              if (confirmation) {
                const saveVersionSubscription = this.api.plans.updateVersion(masterVersion.id, {
                  ...masterVersion,
                  EducationTool3DCVersionId: masterVersion.lastKnown3DCVersion,
                  catalogId: masterVersion?.lastKnownCatalogId,
                  previewPath: masterVersion?.lastKnownPreviewPath,
                  romPath: masterVersion?.lastKnownRomPath,
                  versionNumber: masterVersion.versionNumber == String(0) ? String(1) : masterVersion.versionNumber
                }).subscribe(() => {
                  this.notifications.success(this.saveAutosaveSuccess);
                  saveVersionSubscription.unsubscribe();
                  this.reloadDataView();
                })
              }
            })
        })
      } else {
        const dialog = this.matDialog.open(UnableAutosaveRecoverDialogComponent);
      }
    })
  }

  restoreLastPlanAutosave() {
    if (this.originalPlan.EducationOrigin === EducationToolType.THREE_DC) {
      this.saveLastPlanAutosaveThreeDC();
    } else {
      this.saveLastPlanAutosaveForFusion();
    }
  }

  public saveLastPlanAutosaveForFusion(): void {
    const planCode = this.planDetails.get('planCode').value;
    const readAutosavedRomSubscription = this.recoverPlanAutosaveService.subscribeToPlanAutosaveFile((data) => {
      this.ngZone.run(() => {
        if (data) {
          this.dialogs.confirmation(this.recoverLastAutosaveTitle, this.recoverLastAutosaveMessage + planCode + '?')
            .then((confirmation) => {
              if (confirmation) {
                const romfileName = planCode + '_Emergency_Autosave' + '.rom';
                const romfile = new File(
                  [data as ArrayBuffer],
                  romfileName,
                  { type: 'application/octet-stream' }
                );
                const saveVersionSubscription = this.api.plans.saveVersion(
                  this.currentPlanId,
                  this.createFormData(romfile, romfileName)
                ).subscribe((response) => {
                  this.notifications.success(this.saveAutosaveSuccess);
                  saveVersionSubscription.unsubscribe();
                  this.reloadDataView();
                }, (error) => {
                  this.notifications.error(this.saveAutosaveError);
                });
                this.savedPlanMiddlewareService.resetValues();
              }
            });
        } else {
          const dialog = this.matDialog.open(UnableAutosaveRecoverDialogComponent);
        }
      });
      readAutosavedRomSubscription.unsubscribe();
      this.savedPlanMiddlewareService.resetValues();
    });
    this.middlewareService.recoverPlanAutosave(planCode);
  }

  private createFormData(romfile: File, romfileName: string) {
    const catalogueId = this.planDetails.get('catalogId').value;
    const formData = new FormData();
    formData.append('romFile', romfile, romfileName);
    formData.append('previewFile', new Blob([JSON.stringify(null, null, 2)], { type: 'application/json' }), 'NO_PREVIEW_EMERGENCY_AUTOSAVE.JPEG');
    formData.append('model', JSON.stringify({
      id: 0,
      versionNotes: 'Autosave Recovery',
      catalogCode: this.catalogs.find(catalogItem => catalogItem.id === catalogueId).code,
      range: null,
      romItems: []
    }));
    return formData;
  }

  protected initializeTranslationStrings() {
    const subscription = this.translate.get([
      'notification.markMasterVersionError',
      'notification.archivePlanSuccess',
      'notification.archivePlanError',
      'notification.unarchivePlanSuccess',
      'notification.unarchivePlanError',
      'notification.changesSaved',
      'notification.planUpdatedSuccess',
      'notification.saveError',
      'notification.planUpdatedError',
      'dialog.saveAutosaveSuccess',
      'dialog.saveAutosaveError',
      'dialog.unableToRecoverAutosave.title',
      'dialog.unableToRecoverAutosave.message',
      'dialog.recoverLastAutosave.title',
      'dialog.recoverLastAutosave.message',
      'dialog.versionPublishing.title',
      'dialog.versionPublishing.message',
      'dialog.versionNotPublished.title',
      'dialog.versionNotPublished.message',
      'dialog.versionNotPublished.message2',
      'plan.versionNo',
      'plan.versionId',
      'plan.range',
      'plan.versionNotes',
      'plan.quoteOrderNumber',
      'plan.lastUpdated',
      'publish.jobId',
      'publish.renderType',
      'publish.dateTime',
      'publish.state',
      'publish.publishJobState.Rendering',
      'publish.publishJobState.EmailSent'
    ]).subscribe((translations) => {
      this.markMasterVersionError = translations['notification.markMasterVersionError'];
      this.archivePlanSuccess = translations['notification.archivePlanSuccess'];
      this.archivePlanError = translations['notification.archivePlanError'];
      this.unarchivePlanSuccess = translations['notification.unarchivePlanSuccess'];
      this.unarchivePlanError = translations['notification.unarchivePlanError'];
      this.planChangesSaved = translations['notification.changesSaved'];
      this.planUpdatedSuccess = translations['notification.planUpdatedSuccess'];
      this.planSaveError = translations['notification.saveError'];
      this.planUpdatedError = translations['notification.planUpdatedError'];
      this.saveAutosaveSuccess = translations['dialog.saveAutosaveSuccess'];
      this.saveAutosaveError = translations['dialog.saveAutosaveError'];
      this.unableToRecoverAutosaveTitle = translations['dialog.unableToRecoverAutosave.title'];
      this.unableToRecoverAutosaveMessage = translations['dialog.unableToRecoverAutosave.message'];
      this.recoverLastAutosaveTitle = translations['dialog.recoverLastAutosave.title'];
      this.recoverLastAutosaveMessage = translations['dialog.recoverLastAutosave.message'];
      this.versionPublishingTitle = translations['dialog.versionPublishing.title'];
      this.versionPublishingMessage = translations['dialog.versionPublishing.message'];
      this.versionNotPublishedTitle = translations['dialog.versionNotPublished.title'];
      this.versionNotPublishedMessage = translations['dialog.versionNotPublished.message'];
      this.versionNotPublishedMessage2 = translations['dialog.versionNotPublished.message2'];
      this.versionNumberHeader = translations['plan.versionNo'];
      this.versionIdHeader = translations['plan.versionId'];
      this.rangeHeader = translations['plan.range'];
      this.versionNotesHeader = translations['plan.versionNotes'];
      this.orderNumberHeader = translations['plan.quoteOrderNumber'];
      this.lastUpdatedHeader = translations['plan.lastUpdated'];
      this.renderTypeHeader = translations['publish.renderType'];
      this.dateHeader = translations['publish.dateTime'],
      this.stateHeader = translations['publish.state'];
      this.renderingMsg = translations['publish.publishJobState.Rendering'];
      this.emailSentMsg = translations['publish.publishJobState.EmailSent']
    });
    this.entitySubscriptions.push(subscription);
  }

  private recoverViewData(planId: number): any {
    const zipSubscription =
      zip(
        this.api.plans.getPlan(planId),
        this.api.plans.getPlanActions(planId),
        this.api.plans.getPlanComments(planId),
        this.api.plans.getPlanVersions(planId),
        this.api.publish.getPublishJobsByPlanId(planId)
      )
        .pipe(
          map((apiData) => {
            return {
              plan: apiData[0],
              planActions: apiData[1],
              planComments: apiData[2],
              planVersions: apiData[3],
              publishJobs: apiData[4]
            };
          })
        )
        .subscribe((zipResponse) => {
          this.api.plans.getCatalogs(zipResponse.plan.EducationOrigin)
            .subscribe((catalogs) => {
              this.catalogs = catalogs;
            })
          this.originalPlan = zipResponse.plan;
          this.planVersions = zipResponse.planVersions;
          this.publishJobs = zipResponse.publishJobs;
          this.timelineService.setPlanId({
            masterVersionId: this.originalPlan.id,
            planCode: this.originalPlan.planCode})
          this.planDetails.patchValue(this.originalPlan);
          this.planDetails.get('hasEndUser').patchValue(!!this.originalPlan.endUserId);
          this.displayPlanPreviewContainer();
          if (this.originalPlan.endUserId) {
            this.initializeEndUser();
          }
          if (this.originalPlan.builderId) {
            this.initializeBuilder();
          }
          this.isAssigned = !!this.originalPlan.builderId;
          this.loadPlanVersions(this.originalPlan.EducationOrigin === EducationToolType.THREE_DC ? this.planVersions.filter(pv => pv.versionNumber != String(0)) : this.planVersions);
          if (this.publishJobs) {
            this.loadPlanPublishJobs(this.publishJobs);
          }
          this.getMasterVersionIndex(this.originalPlan.masterVersionId);
          if (this.originalPlan.EducationerId) {
            this.getEducationer(this.originalPlan.EducationerId);
          }
          this.currentPlanMasterVersion = this.getMasterVersion(this.originalPlan.masterVersionId);
          if (this.currentPlanMasterVersion) {
            this.getMasterVersionPublishStatus(this.currentPlanMasterVersion.externalCode);
          }
          const mappedActions = zipResponse.planActions.map((action) => {
            return { type: this.timelineTypeEnum.ACTION, object: action };
          });
          this.timelineItems = []
          this.timelineItems = [...this.timelineItems, ...mappedActions];
          const mappedComments = zipResponse.planComments.map((comment) => {
            return { type: this.timelineTypeEnum.COMMENT, object: comment };
          });
          this.timelineItems = [...this.timelineItems, ...mappedComments];
          this.timelineService.setTimelineItems(this.timelineItems)
        });
    this.entitySubscriptions.push(zipSubscription);
  }

  private initializeEndUser() {
    const endUserSubscription = this.api.plans.getEndUser(this.originalPlan.endUserId)
      .subscribe((response: EndUser) => {
        this.originalPlan.endUser = response;
        this.originalEndUser = response;
        this.endUser = response;
      });
    this.entitySubscriptions.push(endUserSubscription);
  }

  private initializeBuilder() {
    const builderSubscription = this.api.builders.getBuilder(this.originalPlan.builderId)
      .subscribe((response: Builder) => {
        this.originalBuilder = response;
      });
    this.entitySubscriptions.push(builderSubscription);
  }

  private validateExistingEndUser(endUserDetails) {
    const endUserValidationSubscription = this.api.plans.validateExistingEndUsers(endUserDetails)
      .subscribe((response) => {
        const existingEndUser = response.endUser;
        const existingEndUserAiep = response.Aiep;
        if (!existingEndUser && !this.originalPlan.endUserId) {
          this.createNewEndUser(endUserDetails);
        } else if ((!existingEndUser || this.originalPlan.endUserId === existingEndUser.id) && this.originalPlan.endUserId) {
          // No match or match with itself
          this.updateEndUser(endUserDetails);
        } else {
          this.assignExistingEndUser(existingEndUserAiep, existingEndUser, endUserDetails);
        }
      });
    this.entitySubscriptions.push(endUserValidationSubscription);
  }

  private assignExistingEndUser(existingEndUserAiep: any, existingEndUser: any, endUserDetails: any) {
    let existingOtherAiepMsg: string;
    const subscription = this.translate.get([
      'dialog.endUserExistingOtherAiep'
    ], {
      AiepName: existingEndUserAiep ? existingEndUserAiep.name : 'NoAiep'
    }).subscribe((translations) => {
      existingOtherAiepMsg = translations['dialog.endUserExistingOtherAiep'];
      this.dialogs.confirmation('dialog.endUserExistingTitle', existingOtherAiepMsg).then((confirmation) => {
        if (confirmation) {
          this.planDetails.value.endUserId = existingEndUser.id;
          this.submitPlanUpdate(this.planDetails.value);
        }
      });
    });
    this.entitySubscriptions.push(subscription);
  }

  private createNewEndUser(endUserDetails: EndUser): void {
    const endUser = {
      ...this.originalEndUser,
      ...endUserDetails
    }
    const planDetails = {
      ...this.originalPlan,
      endUser: endUser
    }
    this.updatePlan(planDetails);
  }

  private getEducationerFullname(firstName?: string, lastName?: string) {
    let name = firstName ? firstName : '';
    let surname = lastName ? lastName : '';

    return (name || surname) ? `${name} ${surname}` : '-';
  }

  private getEducationer(EducationerId: number) {
    const EducationerSubscription = this.api.users.getUser(EducationerId)
      .subscribe((response) => {
        if (response) {
          this.originalEducationer = {
            ...response,
          fullName: this.getEducationerFullname(response.firstName, response.surname)
        }
        }
      })
    this.entitySubscriptions.push(EducationerSubscription);
  }

  public displayPlanPreviewContainer() {
    if (this.originalPlan.masterVersionId) {
      const versionSubscription = this.api.plans.getVersionById(this.originalPlan.masterVersionId)
        .subscribe((version: Version) => {
          if (version.previewPath) {
            const previewSubscription = this.api.plans.getVersionPreview(this.originalPlan.masterVersionId)
              .pipe(finalize(() => {
                this.loadingImage = false;
              }))
              .subscribe((response: ArrayBuffer) => {
                this.imageBuffer = response;
                this.imageBlob = new Blob([new Uint8Array(response)], { type: 'image/jpeg' });
                this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(this.imageBlob));
              });
            this.entitySubscriptions.push(previewSubscription);
          }
          else {
            this.loadingImage = false;
            this.noImageAvailable = false;
            this.previewUnavailable = true;
          }
        });
      this.entitySubscriptions.push(versionSubscription);
    }
    else {
      this.loadingImage = false;
      this.noImageAvailable = true;
    }
  }

  public openMasterVersionInEducationTool(plan: Plan): void {
    // TODO is this the right place to set the current plan? review it
    this.planDetailsService.setPlanDetails(plan);
    this.EducationToolService.openPlan(plan, plan.EducationOrigin);
  }

  public openVersionPlanInEducationTool(event: MouseEvent, version: Version): void {
    event.stopPropagation();
    event.preventDefault();
    this.EducationToolService.openVersionInEducationTool(version, this.originalPlan.builderId, this.originalPlan.EducationOrigin);
  }

  public publishVersion(versionId: number): void {
    this.dialogs.publish(versionId, this.originalPlan.planCode, this.originalPlan).
      then((confirmation) => {
        if (confirmation) {
          this.masterPublishStatus = PublishStatusEnum.Rendering;
        }
    });
  }

  public publishPlan(plan: Plan): void {
    this.dialogs.openTenderPackPlanPublishModal(plan);
  }

  downloadFittersPackPdf(plan) {
    this.api.plans.downloadFittersPackPdf(plan.id).subscribe((response) => {
      if (response.byteLength > 0) {
        this.dialogs.pdfPreview(response, 'fitters_pack_');
      }
    });
  }

  private getLatestPublishJob(publishJobs: PublishJob[]): PublishJob {
    let sortedJobs: PublishJob[] = [];
    sortedJobs = sortedJobs.concat(publishJobs);
    sortedJobs = sortedJobs.sort((a, b) => new Date(a.updatedDate).getTime() - new Date(b.updatedDate).getTime());
    return sortedJobs.pop();
  }

  private sortPublishJobsDesc(publishJobs: PublishJob[]): PublishJob[] {
    let sortedJobs: PublishJob[] = [];
    sortedJobs = sortedJobs.concat(publishJobs);
    sortedJobs = sortedJobs.sort((a, b) => new Date(b.updatedDate).getTime() - new Date(a.updatedDate).getTime());
    return sortedJobs;
  }

  public getMasterVersionPublishStatus(versionCode: string): void {
    const subscribiton = this.api.publish.getPublishJobsByVersionCode(versionCode)
      .subscribe((data) => {
        if (data.length > 0 ) {
          this.publishJobMasterVersion = this.getLatestPublishJob(data);
          this.masterPublishStatus = this.checkPublishStatus(this.publishJobMasterVersion);
        }
      });
    this.entitySubscriptions.push(subscribiton);
  }

  private checkAssetType(assets: PublishAsset[]): string {
    const videoAsset = assets.find(asset => asset.type === PublishAssetTypeEnum.Video)?.path;
    const imageAsset = assets.find(asset => asset.type === PublishAssetTypeEnum.Image)?.path;
    if (videoAsset) return videoAsset;
    else if (imageAsset) return imageAsset;
    else return null;
  }

  public openAssetUrl(publishJob: PublishJob): void {
    //TODO: Uncomment everything it this function when Notification System working, so it allows to only display assests when EmailSent (not when EmailError)
    //const publishJobStatus = this.checkPublishStatus(publishJob);
    const assetUrl = this.checkAssetType(publishJob.assets);
    if (assetUrl /*&& publishJobStatus === PublishStatusEnum.Sucess*/) {
      window.open(assetUrl, '_blank', 'nodeIntegration=no');
    } else {
      this.dialogs.simpleInformation(this.versionPublishingTitle, this.versionPublishingMessage);
    }
  }

  public openMyKitchenUrl(publishJob: PublishJob): void {
    const publishJobStatus = this.checkPublishStatus(publishJob);
    if (publishJobStatus === PublishStatusEnum.Success) {
      const myKitchenPath = publishJob.assets.find(asset => asset.type === PublishAssetTypeEnum.MyKitchen).path;
      window.open(myKitchenPath, '_blank', 'nodeIntegration=no');
    } else {
      this.dialogs.simpleInformation(this.versionPublishingTitle, this.versionPublishingMessage);
    }
  }

  public openMyKitchenMasterVersion() : void {
    //TODO: when myKitchenUrl working change openAssetUrl to openMyKitchenUrl
    //this.openMyKitchenUrl(this.publishJobMasterVersion);
    this.openAssetUrl(this.publishJobMasterVersion);
  }

  public openMyKitchenByVersionCode(versionCode: string): void {
    const subscribiton = this.api.publish.getPublishJobsByVersionCode(versionCode)
      .subscribe((data) => {
        if (data.length > 0) {
          const latestVersion = this.getLatestPublishJob(data);
          //TODO: when myKitchenUrl working change openAssetUrl to openMyKitchenUrl
          //this.openMyKitchenUrl(latestVersion);
          this.openAssetUrl(latestVersion);
        } else {
          this.dialogs.simpleInformation(this.versionNotPublishedTitle, this.versionNotPublishedMessage, this.versionNotPublishedMessage2);
        }
      })
      this.entitySubscriptions.push(subscribiton);
  }

  private checkPublishStatus(publishJob: PublishJob): PublishStatusEnum {
    if (RenderingStatus.some(item => item === publishJob.stateName)) {
      return PublishStatusEnum.Rendering;
    }
    if (publishJob.stateName === SuccessStatus) {
      return PublishStatusEnum.Success;
    }
      return PublishStatusEnum.Error;
  }

  public updateNotesAndQuote(event: any) {
    const notesSubscription = this.api.plans.modifyVersionNotesAndQuote(
      this.originalPlan.masterVersionId,
      event.versionNotes,
      event.quoteOrderNumber
    ).subscribe();
    this.entitySubscriptions.push(notesSubscription);
  }

  private updateEndUser(endUserDetails: EndUser) {
    const planDetails = {
      ...this.originalPlan,
      endUser: {
        ...this.originalEndUser,
        ...endUserDetails
      }
    };
    this.updatePlan(planDetails);
  }

  private updatePlan(planDetails: Plan) {
    const updateSubscription = this.api.plans.updatePlan({
      ...this.originalPlan,
      ...planDetails
    }).subscribe((planResponse: Plan) => {
      this.originalEndUser = planResponse.endUser;
      this.notifications.success(this.planChangesSaved, this.planUpdatedSuccess);
      this.reloadDataView();
    }, (error) => {
      this.notifications.error('notification.saveError', 'notification.planUpdatedError');
    });
    this.entitySubscriptions.push(updateSubscription);
  }

  private initializeColumnConfiguration() {
    this.versionsColumnNames = [
      { columnDef: 'versionNumber', header: this.versionNumberHeader },
      { columnDef: 'externalCode', header: this.versionIdHeader, tooltipAtLength: 8 },
      { columnDef: 'range', header: this.rangeHeader, tooltipAtLength:  10 },
      { columnDef: 'versionNotes', header: this.versionNotesHeader, tooltipAtLength: 7 },
      { columnDef: 'quoteOrderNumber', header: this.orderNumberHeader, tooltipAtLength: 9 },
      { columnDef: 'updatedDate', header: this.lastUpdatedHeader, isDate: true },
    ];
    this.publishJobsColumnNames = [
      { columnDef: 'versionCode', header: this.versionIdHeader, tooltipAtLength: 8 },
      { columnDef: 'renderingType', callback: (record: any) => this.translateFields(record.renderingType, 'publish.renderTypeOptions.'), header: this.renderTypeHeader, tooltipAtLength: 8 },
      { columnDef: 'updatedDate', header: this.dateHeader, tooltipAtLength: 35, isDateAndTime: true },
      { columnDef: 'stateName', callback: (record: any) => this.getRenderingTypeName(record.stateName, 'publish.publishJobState.'), header: this.stateHeader, tooltipAtLength: 10 }
    ];
  }

  private getRenderingTypeName(renderingType: string, translationKey: string): string {
    if (RenderingStatus.some(item => item === renderingType)) {
      return this.renderingMsg;
    }
    if (renderingType === SuccessStatus) {
      return this.emailSentMsg;
    }
    return this.translateFields(renderingType, translationKey);
  }

  private translateFields(field: string | number, translationKey: string): string {
    const translationField = this.translate.instant(translationKey + field);
    return translationField;
  }

  public openTimeline() {
    this.sidebarService.getSidebar(this.sidebar).open();
  }

  public openCreatePlanModal() {
    this.planDetailsService.openMainPlanDetails(this.countryService);
  }

  public openPlanPreview(record: any): void {
    const data: PlanPreviewComponentData = {
      versionId: record.id,
      plan: this.originalPlan,
      showPromoteMaster: true,
      isMasterVersion: this.checkVersionStatus(record),
      planVersions: this.planVersions,
      showButton: false
    }
    this.dialogsPlanPreview.planPreview(data).then((response) => {
      if (response === true) {
        this.reloadDataView();
      }
    });
  }

  public getCurrentTabIndex(tabNumber: number) {
    if (this.originalPlan && tabNumber !== 0) {
      this.reloadDataView();
    }

    this.showRestoreButton = tabNumber === 1;
  }

  protected readonly EducationToolType = EducationToolType;
}


