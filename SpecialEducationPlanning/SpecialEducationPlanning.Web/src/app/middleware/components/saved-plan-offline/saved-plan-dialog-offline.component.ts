import { Component, Inject, OnInit, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Validators } from '@angular/forms';

import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';

import { OMConstants } from 'offline-middleware';
import { FormComponent } from '../../../shared/base-classes/form-component';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { VersionOffline, PlanOffline } from '../../../shared/models/plan-offline';
import { OfflineMiddlewareService } from '../../services/offline-middleware.service';
import { ActionTypeOfflineEnum } from '../../../shared/models/app-enums';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { FusionMiddlewareResponse } from '../../models/FusionMiddlewareResponse';

@Component({
  selector: 'tdp-offline-middleware-saved-plan-dialog',
  templateUrl: 'saved-plan-dialog-offline.component.html',
  styleUrls: ['saved-plan-dialog-offline.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class OfflineMiddlewareSavedPlanDialogComponent extends FormComponent implements OnInit, AfterViewInit {

  public readonly maxVersionNotesLength: number = 500;
  public readonly maxVersionQuoteNumberLength: number = 20;
  public isNewPlan: boolean;
  public planName: string;
  public planVersionIdOffline: number;
  public fusionData: any;
  public planOffline: PlanOffline;

  // Translation strings
  public newPlanTitleLabel = '';
  public versionModifiedTitleLabel = '';
  public discardPlanButtonText = '';
  public discardChangesNotification = '';
  public saveNewPlanNotification = '';
  public overWritePlanNotification = '';

  constructor(
    private dialogRef: MatDialogRef<OfflineMiddlewareSavedPlanDialogComponent>,
    private dialogs: DialogsService,
    private translate: TranslateService,
    private offlineMiddleware: OfflineMiddlewareService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) private data: FusionMiddlewareResponse,
    private communication: CommunicationService
  ) {
    super();
    this.isNewPlan = false;
    this.dialogRef.disableClose = true;
    this.fusionData = this.data; // Not using MiddlewareResponse interface because it contains id_offline
    this.planVersionIdOffline = +(this.fusionData.planVersionId ? this.fusionData.planVersionId : '');
    this.entityForm = this.formBuilder.group({
      versionNotes: ['', Validators.maxLength(this.maxVersionNotesLength)],
      quoteOrderNumber: ['', Validators.maxLength(this.maxVersionQuoteNumberLength)]
    });
    this.planName = '';
  }

  ngOnInit(): void {
    this.isNewPlan = this.fusionData.isNewPlan;
    this.planName = this.fusionData.planName;
    this.initializeVersionNotesControl();
    this.getTranslationStrings();
  }

  ngAfterViewInit(): void {
    this.getPlan();
  }

  public discardVersion(): void {
    this.dialogs.confirmation(
      'dialog.discardPlanVersion',
      'dialog.discardPlanMessage'
    ).then((confirmation) => {
      if (confirmation) {
        this.moveVersionFiles(OMConstants.PLACE_VERSION_ACTIONS.DISCARD, 0, this.planOffline.planNumber);
        this.notifications.success(this.discardChangesNotification);
        this.offlineMiddleware.createActionLog(
          this.offlineMiddleware.getPromiseManager().getHostname(),
          ActionTypeOfflineEnum.Discard,
          0,
          '',
          false
        );
        this.dialogRef.close();
      }
    });
  }

  // New Plan
  public saveNewPlanVersion(): void {
    const submittedVersionNotes = this.entityForm.controls['versionNotes'].value;
    this.dialogRef.close(submittedVersionNotes);
    const updatedPlan = {
      ...this.planOffline,
      versionNotes: submittedVersionNotes,
      quoteOrderNumber: this.entityForm.controls['quoteOrderNumber'].value
    };
    updatedPlan.versions.push(this.generateVersionModel(this.fusionData, updatedPlan, true));
    this.moveVersionFiles(OMConstants.PLACE_VERSION_ACTIONS.CREATE, 1, updatedPlan.planNumber);
    this.updatePlanWithVersion(updatedPlan, this.newPlanTitleLabel, ActionTypeOfflineEnum.Create);
  }

  // New version for existing plan
  public saveNewVersion(): void {
    const version = this.generateVersionModel(this.fusionData, this.planOffline, true);
    this.planOffline.versions.push(version);
    this.moveVersionFiles(OMConstants.PLACE_VERSION_ACTIONS.CREATE, version.id_offline, this.planOffline.planNumber);
    this.updatePlanWithVersion(this.planOffline, this.saveNewPlanNotification, ActionTypeOfflineEnum.SaveNew);
  }

  // Update version
  public overwriteVersion(): void {
    const version = this.generateVersionModel(this.fusionData, this.planOffline, false);
    const planIndex = this.planOffline.versions.findIndex(p => p.id_offline === version.id_offline);
    this.planOffline.versions[planIndex] = version;
    this.moveVersionFiles(OMConstants.PLACE_VERSION_ACTIONS.OVERWRITE, version.id_offline, this.planOffline.planNumber);
    this.updatePlanWithVersion(this.planOffline, this.overWritePlanNotification, ActionTypeOfflineEnum.Overwrite);
  }

  private generateVersionModel(fusionModel: FusionMiddlewareResponse, planModel: PlanOffline, isCreate: boolean): VersionOffline {
    let versionId = 0;
    if (isCreate) {
      const planVersions = planModel.versions;
      versionId = planVersions && planVersions.length > 0 ? planVersions.length + 1 : 1;
    } else {
      versionId = fusionModel.planVersionId;
    }
    return {
      id_offline: versionId,
      romPath: fusionModel.romFileInfo.fileName.endsWith('.Rom') ?
        fusionModel.romFileInfo.fileName :
        fusionModel.romFileInfo.fileName + '.Rom',
      previewPath: fusionModel.preview.fileName,
      versionNotes: this.entityForm.controls['versionNotes'].value,
      quoteOrderNumber: this.entityForm.controls['quoteOrderNumber'].value,
      catalogueCode: fusionModel.catalogType,
      range: fusionModel.mainRange,
      romItems: fusionModel.lineItems,
      updatedDate: new Date(),
      createdDate: new Date(),
    };
  }

  private initializeVersionNotesControl() {
    const versionNotesControl = this.entityForm.controls['versionNotes'];
    versionNotesControl.setValue(this.fusionData.versionNotes ? this.fusionData.versionNotes : '');
    const quoteOrderNumberControl = this.entityForm.controls['quoteOrderNumber'];
    quoteOrderNumberControl.setValue(this.fusionData.quoteOrderNumber ? this.fusionData.quoteOrderNumber : '');
  }

  private getTranslationStrings() {
    const translateSubscription = this.translate.get([
      'dialog.newPlanSaved',
      'dialog.versionModified',
      'button.discardChanges',
      'button.saveNewPlan',
      'dialog.overwritePlanSuccess',
    ], {
        planId: this.fusionData.planId,
        versionNumber: this.planVersionIdOffline
      }).subscribe((translations) => {
        this.newPlanTitleLabel = translations['dialog.newPlanSaved'];
        this.versionModifiedTitleLabel = translations['dialog.versionModified'];
        this.discardChangesNotification = translations['button.discardChanges'];
        this.saveNewPlanNotification = translations['button.saveNewPlan'];
        this.overWritePlanNotification = translations['dialog.overwritePlanSuccess'];
      });
    this.entitySubscriptions.push(translateSubscription);
  }

  private moveVersionFiles(action: any, version: number, planNumber: string): void {
    const data = {
      action: action,
      versionNumber: version,
      planNumber: planNumber
    };

    this.offlineMiddleware.moveVersionFiles(data);
  }

  private getPlan(): void {
    const getPlanSubscription = this.offlineMiddleware.getSinglePlanObservable(this.fusionData.id_offline)
      .subscribe((plan: PlanOffline) => {
        this.planOffline = plan;
      });
    this.entitySubscriptions.push(getPlanSubscription);
  }

  private updatePlanWithVersion(updatedPlan: PlanOffline, notificationText: string, actionType: ActionTypeOfflineEnum) {
    const updatePlanWithVersionSub = this.offlineMiddleware
      .editPlanObservable(updatedPlan).subscribe((planUpdatedWithVersion: PlanOffline) => {
        this.notifications.success(notificationText);
        this.offlineMiddleware.createActionLog(
          this.offlineMiddleware.getPromiseManager().getHostname(),
          actionType,
          planUpdatedWithVersion.id_offline,
          updatedPlan.planNumber,
          this.isNewPlan
        );
        this.communication.notifyReloadViewData();
        this.dialogRef.close();
      });
    this.entitySubscriptions.push(updatePlanWithVersionSub);
  }

}
