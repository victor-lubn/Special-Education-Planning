import { Component, NgZone, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { DocumentModel } from '../../../../middleware/models/document.model';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { BaseComponent } from '../../../../shared/base-classes/base-component';
import { PlanPreviewOfflineDialogComponent } from '../../../../shared/components/dialogs/plan-preview-offline-dialog/plan-preview-offline-dialog.component';
import { SimpleInformationDialogComponent } from '../../../../shared/components/dialogs/simple-information-dialog/simple-information-dialog.component';
import { OfflineDetailsContainerLeftHandSideComponent } from '../../../../shared/components/organisms/offline-details-container-left-hand-side -/offline-details-container-left-hand-side.component';
import { TableColumnConfig, TableRecords } from '../../../../shared/components/organisms/table/table.types';
import { ComponentCanDeactivate } from '../../../../shared/guards/pending-changes.guard';
import { ActionTypeOfflineEnum } from '../../../../shared/models/app-enums';
import { PlanOffline, VersionOffline } from '../../../../shared/models/plan-offline';

@Component({
  selector: 'tdp-plan-details-offline',
  templateUrl: './plan-details-offline.component.html',
  styleUrls: ['./plan-details-offline.component.scss']
})
export class PlanDetailsOfflineComponent extends BaseComponent implements ComponentCanDeactivate, OnInit {

  public plan: PlanOffline;
  public planVersions: VersionOffline[] = [];

  private planIdOffline: number;
  public pageSize = 14;
  public versionsTable: TableRecords<VersionOffline> = { data: [] };
  public offlineVersionsColumnNames: TableColumnConfig[] = [];

  //Translation strings
  private updateSuccess: string = '';
  private versionNumberHeader: string = '';
  private rangeHeader: string = '';
  private versionNotesHeader: string = '';
  private orderNumberHeader: string = '';
  private lastUpdated: string = '';

  @ViewChild(OfflineDetailsContainerLeftHandSideComponent)
  private planOfflineForm: OfflineDetailsContainerLeftHandSideComponent;

  constructor(
    private activatedRoute: ActivatedRoute,
    private offlineMiddleware: OfflineMiddlewareService,
    private communication: CommunicationService,
    private fusionMiddleware: EducationToolMiddlewareService,
    private notifications: NotificationsService,
    private translate: TranslateService,
    private dialogs: DialogsService,
    private matDialog: MatDialog,
    private ngZone: NgZone
  ) {
    super();
  }

  ngOnInit() {
    this.getTranslationStrings();
    this.initializeColumnConfiguration();
    this.getPlanNumberRouteParameter();
    this.initializeReloadViewData();
  }

  public planOfflineSubmittedHandler(offlinePlanSubmitted: PlanOffline): void {
    const planEditedSubscription = this.offlineMiddleware
      .editPlanObservable({
        ...this.plan,
        ...offlinePlanSubmitted
      })
      .subscribe((planEdited: PlanOffline) => {
        this.plan = planEdited;
        this.planVersions = planEdited.versions ? planEdited.versions : [];
        this.notifications.success(this.updateSuccess);

        this.offlineMiddleware.createActionLog(
          this.fusionMiddleware.getPromiseManager().getHostname(),
          ActionTypeOfflineEnum.Update,
          this.plan.id_offline,
          this.plan.planNumber,
          true
        );
        planEditedSubscription.unsubscribe();
      });
    this.entitySubscriptions.push(planEditedSubscription);
  }

  hasChanges() {
    return this.planOfflineForm.child.entityForm.dirty;
  }

  private getPlan(idOffline: number): void {
    const getPlanSubscription = this.offlineMiddleware.getSinglePlanObservable(idOffline).subscribe((planResponse) => {
      this.ngZone.run(() => {
        if (planResponse && planResponse.id_offline === idOffline) {
          this.plan = planResponse;
          this.planVersions = planResponse.versions ? planResponse.versions : [];
          this.versionsTable = { data: this.planVersions, total: this.planVersions.length };
        } else {
          console.log('readFileError', 'readFileErrorMessage');
        }
      });
    });
    this.entitySubscriptions.push(getPlanSubscription);
  }

  private getPlanNumberRouteParameter() {
    const routerSubscription = this.activatedRoute.params.subscribe((params) => {
      this.planIdOffline = +params['id'];
      this.getPlan(this.planIdOffline);
    });
    this.entitySubscriptions.push(routerSubscription);
  }

  private getTranslationStrings() {
    const translateSubscription = this.translate.get([
      'notification.planUpdatedSuccess',
      'plan.versionNo',
      'plan.range',
      'plan.versionNotes',
      'plan.quoteOrderNumber',
      'plan.lastUpdated'
    ]).subscribe((translations) => {
      this.updateSuccess = translations['notification.planUpdatedSuccess'];
      this.versionNumberHeader = translations['plan.versionNo'];
      this.rangeHeader = translations['plan.range'];
      this.versionNotesHeader = translations['plan.versionNotes'];
      this.orderNumberHeader = translations['plan.quoteOrderNumber'];
      this.lastUpdated = translations['plan.lastUpdated'];
    });
    this.entitySubscriptions.push(translateSubscription);
  }

  private initializeReloadViewData() {
    const reloadViewSubscription = this.communication.subscribeToReloadViewData(() => {
      this.getPlan(this.planIdOffline);
    });
    this.entitySubscriptions.push(reloadViewSubscription);
  }

  goBack() {
    if (this.hasChanges()) {
      this.dialogs.confirmation('dialog.genericUnsavedChanges', 'dialog.genericCancelDialog').then((success) => {
        if (success) {
          this.navigateTo('/offline/planOffline')
        }
      })
    }
  }

  openCreatePlanOfflineModal() {
    this.dialogs.openCreatePlanOfflineModal()
  }

  private initializeColumnConfiguration() {
    this.offlineVersionsColumnNames = [
      { columnDef: 'id_offline', header: this.versionNumberHeader },
      { columnDef: 'range', header: this.rangeHeader, tooltipAtLength: 12 },
      { columnDef: 'versionNotes', header: this.versionNotesHeader, tooltipAtLength: 14 },
      { columnDef: 'quoteOrderNumber', header: this.orderNumberHeader, tooltipAtLength: 13 },
      { columnDef: 'updatedDate', header: this.lastUpdated, isDate: true }
    ];
  }

  public openPlanPreview(event: MouseEvent, record: VersionOffline) {
    event.stopPropagation();
    event.preventDefault();
    const dialogRef = this.matDialog.open(PlanPreviewOfflineDialogComponent, {
      data: {
        version: record,
        plan: this.plan,
        showButton: false
      }
    });
  }

  public updateNotesAndQuote(record: VersionOffline) {
    this.dialogs.editVersionNotesOffline(record, this.plan.id_offline);
  }

  public openVersionInFusion(event: MouseEvent, version: VersionOffline) {
    event.stopPropagation();
    event.preventDefault();
    const getRomFileSubscription = this.offlineMiddleware.getRomFileObservable(version.romPath)
      .subscribe((data) => {
        if (data) {
          const romFile = {
            type: 'application/octet-stream',
            fileName: this.plan.planNumber,
            romByteArray: data
          };
          const model = this.generateFusionModel(version, romFile);
          this.fusionMiddleware.openDocument(model);
        } else {
          this.ngZone.run(() => {
            const dialog = this.matDialog.open(SimpleInformationDialogComponent, {
              width: '32em',
              data: {
                titleStringKey: 'dialog.file.readFileError',
                messageStringKey: 'dialog.file.readFileErrorMessage'
              }
            });
          });
        }
      });
    this.entitySubscriptions.push(getRomFileSubscription);
  }

  private generateFusionModel(version: VersionOffline, romFile: any) {
    return new DocumentModel(
      false,
      this.plan.id_offline,
      this.plan.planNumber,
      version.catalogueCode,
      null,
      null,
      version.id_offline,
      version.id_offline.toString(),
      romFile,
      version.versionNotes,
      version.quoteOrderNumber,
      null,
      this.plan.planName,
      null,
      this.plan.id_offline
    );
  }
}

