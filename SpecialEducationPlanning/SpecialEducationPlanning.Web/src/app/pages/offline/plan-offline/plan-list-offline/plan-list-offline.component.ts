import { Component, OnInit, NgZone } from '@angular/core';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { PlanOffline, VersionOffline } from '../../../../shared/models/plan-offline';
import { BaseComponent } from '../../../../shared/base-classes/base-component';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { ActivatedRoute } from '@angular/router';
import { TableColumnConfig, TableRecords } from '../../../../shared/components/organisms/table/table.types';
import { PlanPreviewOfflineDialogComponent } from '../../../../shared/components/dialogs/plan-preview-offline-dialog/plan-preview-offline-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SimpleInformationDialogComponent } from '../../../../shared/components/dialogs/simple-information-dialog/simple-information-dialog.component';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { DocumentModel } from '../../../../middleware/models/document.model';

@Component({
  selector: 'tdp-plan-list-offline',
  templateUrl: './plan-list-offline.component.html',
  styleUrls: ['./plan-list-offline.component.scss']
})
export class PlanListOfflineComponent extends BaseComponent implements OnInit {

  public offlinePlanList: TableRecords<PlanOffline> = { data: [] };

  private readFileError: string;
  private readFileErrorMessage: string;
  private planResults: string;
  public columnsConfigurationForOfflinePlans: TableColumnConfig[];
  private planId: string;
  private planName: string;
  private EducationerName: string;
  private createdAt: string;
  public pageSize: number = 7;

  constructor(
    private offlineMiddleware: OfflineMiddlewareService,
    private dialogs: DialogsService,
    private matDialog: MatDialog,
    private ngZone: NgZone,
    private notificationService: NotificationsService,
    private translate: TranslateService,
    private communication: CommunicationService,
    private fusionMiddleware: EducationToolMiddlewareService,
    private activatedRouter: ActivatedRoute
  ) {
    super();
  }

  ngOnInit() {
    this.getTranslationStrings();
    this.getPlans();
    this.initializeReloadViewData();
    this.initializeColumnsConfiguration()
  }

  public goToCreateNewPlanOffline(): void {
    this.navigateTo('/offline/planOffline/new');
  }

  private getTranslationStrings() {
    const translateSubscription = this.translate.get([
      'dialog.file.readFileError',
      'dialog.file.readFileErrorMessage',
      'offline.list.none',
      'offline.planForm.planId',
      'offline.planForm.planName',
      'offline.planForm.EducationerName',
      'offline.planForm.createdAt',
    ]).subscribe((translations) => {
      this.readFileError = translations['dialog.file.readFileError'];
      this.readFileErrorMessage = translations['dialog.file.readFileErrorMessage'];
      this.planResults = translations['offline.list.none'];
      this.planId = translations['offline.planForm.planId'];
      this.planName = translations['offline.planForm.planName'];
      this.EducationerName = translations['offline.planForm.EducationerName'];
      this.createdAt = translations['offline.planForm.createdAt'];
    });
    this.entitySubscriptions.push(translateSubscription);
  }

  private initializeColumnsConfiguration() {
    this.columnsConfigurationForOfflinePlans = [
      { columnDef: 'id_offline', header: this.planId },
      { columnDef: 'planName', header: this.planName, tooltipAtLength: 15 },
      { columnDef: 'EducationerName', header: this.EducationerName, tooltipAtLength: 15 },
      { columnDef: 'createdDate', header: this.createdAt, isDate: true }
    ];
  }

  private initializeReloadViewData() {
    const reloadDataViewSubscription = this.communication.subscribeToReloadViewData(() => {
      this.getPlans();
    });
    this.entitySubscriptions.push(reloadDataViewSubscription);
  }

  public goToPlanDetails(plan: PlanOffline) {
    this.navigateTo('/offline/planOffline/' + plan.id_offline);
  }

  private getPlans(): void {
    const readPlansSubscription = this.offlineMiddleware.readPlansObservable()
      .subscribe((fileList) => {
        this.ngZone.run(() => {
          if (fileList && fileList.obj && fileList.obj.length > 0) {
            // TODO return obj instead of filelist from subscribe
            this.offlinePlanList = { data: fileList.obj, total: fileList.obj.length }
          } else if (fileList === '') {
            this.notificationService.warn(this.planResults);
          } else {
            this.dialogs.information(this.readFileError, this.readFileErrorMessage);
          }
        });
      });
    this.entitySubscriptions.push(readPlansSubscription);
  }

  openCreatePlanOfflineModal() {
    this.dialogs.openCreatePlanOfflineModal()
  }

  openLatestVersionPreview(event: MouseEvent, plan: PlanOffline) {
    event.stopPropagation();
    event.preventDefault();
    const latestVersion = this.getLatestVersion(plan);
    this.ngZone.run(() => {
      const dialogRef = this.matDialog.open(PlanPreviewOfflineDialogComponent, {
        data: {
          version: latestVersion,
          plan: plan,
          showButton: true
        }
      });
    });
  }

  openLatestVersionInFusion(event: MouseEvent, plan: PlanOffline) {
    event.stopPropagation();
    event.preventDefault();
    const latestVersion: VersionOffline = this.getLatestVersion(plan);
    this.openPlanInFusion(latestVersion, plan);
  }

  private openPlanInFusion(version: VersionOffline, plan: PlanOffline) {
    const getRomFileSubscription = this.offlineMiddleware.getRomFileObservable(version.romPath)
      .subscribe((data) => {
        if (data) {
          const romFile = {
            type: 'application/octet-stream',
            fileName: plan.planNumber,
            romByteArray: data
          };
          const model = this.generateFusionModel(plan, version, romFile);
          this.fusionMiddleware.openDocument(model);
          this.goToPlanDetails(plan);
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
      },(error) => {
        this.notificationService.error(this.readFileError);
      });
    this.entitySubscriptions.push(getRomFileSubscription);
  }

  private getLatestVersion(plan: PlanOffline): VersionOffline {
    let sortedVersions: VersionOffline[] = [];
    sortedVersions = sortedVersions.concat(plan.versions);
    sortedVersions.sort((a, b) => new Date(a.updatedDate).getTime() - new Date(b.updatedDate).getTime());
    return sortedVersions.pop();
  }

  private generateFusionModel(plan: PlanOffline, version: VersionOffline, romFile: any) {
    return new DocumentModel(
      false,
      plan.id_offline,
      plan.planNumber,
      version.catalogueCode,
      null,
      null,
      version.id_offline,
      version.id_offline.toString(),
      romFile,
      version.versionNotes,
      version.quoteOrderNumber,
      null,
      plan.planName,
      null,
      plan.id_offline
    );
  }

}

