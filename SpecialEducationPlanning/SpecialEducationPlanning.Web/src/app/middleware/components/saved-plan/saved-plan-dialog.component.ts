import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Validators } from '@angular/forms';
import { catchError, finalize, map, switchMap, tap } from 'rxjs/operators';

import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';

import { ApiService } from '../../../core/api/api.service';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { FormComponent } from '../../../shared/base-classes/form-component';
import { PlanDetailsService } from '../../../shared/services/plan-details.service';
import { EducationToolType } from '../../../shared/models/app-enums';
import { ThreeDCMiddlewareResponse } from '../../models/ThreeDCMiddlewareResponse';
import { FusionMiddlewareResponse } from '../../models/FusionMiddlewareResponse';
import { EMPTY, Observable, of } from 'rxjs';
import { Version } from '../../../shared/models/version';
import { Plan } from "../../../shared/models/plan";
import { Catalog } from '../../../shared/models/catalog.model';
import { MiddlewareSavedPlanService } from '../../services/fusion-callbacks/saved-plan.service';

@Component({
  selector: 'tdp-middleware-saved-plan-dialog',
  templateUrl: 'saved-plan-dialog.component.html',
  styleUrls: ['saved-plan-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MiddlewareSavedPlanDialogComponent extends FormComponent implements OnInit {

  public readonly maxVersionNotesLength = 500;
  public readonly maxVersionQuoteNumberLength = 20;
  public isNewPlan: boolean;
  public isTemplate: boolean;
  public EducationOrigin: EducationToolType;
  public builderName: string;
  public planVersionNumber: number;
  public quoteOrderNumber: string;
  public catalogType: string;

  // Translation strings
  public newPlanTitleLabel: string = '';
  public newTemplateTitleLabel: string = '';
  public versionModifiedTitleLabel: string = '';
  public discardPlanButtonText: string = '';
  public discardTemplateButtonText: string = '';
  public discardChangesButtonText: string = '';
  public newVersionButtonText: string = '';
  public newPlanButtonText: string = '';
  public newTemplateButtonText: string = '';
  private overwriteSuccessMessage: string = '';
  private overwriteErrorMessage: string = '';
  private saveAsNewSuccessMessage: string = '';
  private saveAsNewErrorMessage: string = '';
  private unassignedBuilder: string = '';

  constructor(
    private dialogRef: MatDialogRef<MiddlewareSavedPlanDialogComponent>,
    private dialogs: DialogsService,
    private communication: CommunicationService,
    private translate: TranslateService,
    private readonly savedPlanService: MiddlewareSavedPlanService,
    private notifications: NotificationsService,
    private planDetailsService: PlanDetailsService,
    private api: ApiService,
    @Inject(MAT_DIALOG_DATA) private middlewareResponse: ThreeDCMiddlewareResponse | FusionMiddlewareResponse,
  ) {
    super();
    this.isNewPlan = false;
    this.isTemplate = false;
    this.dialogRef.disableClose = true;
    this.entityForm = this.formBuilder.group({
      versionNotes: ['', Validators.maxLength(this.maxVersionNotesLength)],
      quoteOrderNumber: ['', Validators.maxLength(this.maxVersionQuoteNumberLength)]
    });
  }

  ngOnInit(): void {
    this.isNewPlan = this.middlewareResponse.isNewPlan;
    this.isTemplate = this.middlewareResponse.isTemplate;
    this.EducationOrigin = this.middlewareResponse.EducationOrigin;
    this.planVersionNumber = this.middlewareResponse.versionNumber;
    this.getTranslationStrings();
    this.builderName = this.middlewareResponse.builderName ? this.middlewareResponse.builderName : this.unassignedBuilder;
    this.initializeFormFields();
  }

  public discardVersion(): void {
    this.dialogs.confirmation(
      this.isTemplate ? 'dialog.discardTemplateVersion' : 'dialog.discardPlanVersion',
      this.isNewPlan ? (this.isTemplate ? 'dialog.discardNewTemplateMessage' : 'dialog.discardNewPlanMessage') : 'dialog.discardPlanMessage'
    ).then((confirmation) => {
      if (confirmation) {
        if (this.EducationOrigin !== EducationToolType.FUSION) {
          this.dialogRef.close();
        } else {
          this.dialogRef.close();
        }
      }
    });
  }

  public saveVersion(newVersion: boolean): void {
    const resp = this.middlewareResponse as ThreeDCMiddlewareResponse;
    if (this.EducationOrigin === EducationToolType.FUSION) {
      this.saveVersionFusion(resp, newVersion);
    } else {
      this.saveVersion3DC(resp, newVersion);
    }
  }

  private saveVersion3DC(resp: ThreeDCMiddlewareResponse, newVersion: boolean): void {
    this.api.plans.getCatalogByNameAndEducationOrigin(resp.catalogName, EducationToolType.THREE_DC).pipe(
      switchMap((catalog) => {
        resp.catalogId = catalog.id;
        return this.api.plans.getPlan(resp.planId).pipe(
          switchMap((plan) => this.ensureVersionLoadedAndHandleSave(plan, catalog, resp, newVersion))
        );
      })
    ).subscribe({
      next: () => {
        this.communication.notifyReloadViewData();
        this.dialogRef.close();
      },
      error: () => {
        this.notifications.error('An error occurred while processing the request.');
      }
    });
  }

  private ensureVersionLoadedAndHandleSave(
    plan: Plan,
    catalog: Catalog,
    resp: ThreeDCMiddlewareResponse,
    newVersion: boolean
  ): Observable<any> {
    const loadVersion$ = resp.version
      ? of(resp.version)
      : this.api.plans.getPlan(plan.id).pipe(
        map((loadedPlan) => this.savedPlanService.findMasterVersion(loadedPlan))
      );

    return loadVersion$.pipe(
      switchMap((version) => {
        const isMasterVersion = version.id === plan.masterVersionId;
        const catalogNeedsUpdate = plan.catalogId !== catalog.id;

        const updatePlan$ = isMasterVersion && catalogNeedsUpdate && (!newVersion || resp.isNewPlan)
          ? this.api.plans.updatePlan({ ...plan, catalogId: catalog.id }).pipe(
              tap(() => this.communication.notifyReloadViewData())
            )
          : of(null);

        return updatePlan$.pipe(
          switchMap(() => this.handleVersionSave(plan, resp, newVersion))
        );
      })
    );
  }

  private handleVersionSave(plan: Plan, resp: ThreeDCMiddlewareResponse, newVersion: boolean): Observable<any> {
    if (this.isNewPlan) {
      return this.handleNewPlan(resp);
    }

    if (!newVersion) {
      const updatedVersion = this.buildVersionData(resp.version, resp, resp.version.versionNumber);
      return this.updateVersion(updatedVersion, this.overwriteSuccessMessage);
    } else {
      return this.api.plans.getPlan(plan.id).pipe(
        map((retrievedPlan) => this.buildVersionData(null, resp, String(retrievedPlan.versions.length + 1))),
        switchMap((newVersionData) => this.createNewVersion(newVersionData))
      );
    }
  }

  private handleNewPlan(resp: ThreeDCMiddlewareResponse): Observable<any> {
    return this.api.plans.getPlan(resp.planId).pipe(
      map((plan) => {
        const baseVersion = plan.versions.find((v) => v.versionNumber == String(0));
        return this.buildVersionData(baseVersion, resp, String(1));
      }),
      switchMap((updatedVersion) => this.api.plans.updateVersion(updatedVersion.id, updatedVersion))
    );
  }

  private buildVersionData(
    baseVersion: Version | null,
    resp: ThreeDCMiddlewareResponse,
    versionNumber: string
  ): Version {
    return {
      ...baseVersion,
      versionNumber: versionNumber,
      EducationTool3DCVersionId: Number(resp.version3DC),
      lastKnown3DCVersion: Number(resp.version3DC),
      catalogId: resp.catalogId,
      lastKnownCatalogId: resp.catalogId,
      previewPath: resp.thumbnailUrl,
      lastKnownPreviewPath: resp.thumbnailUrl,
      romPath: resp.renderRequestJsonUrl,
      lastKnownRomPath: resp.renderRequestJsonUrl,
      planId: resp.planId
    };
  }

  private updateVersion(version: Version, successMessage: string): Observable<any> {
    return this.api.plans.updateVersion(version.id, version).pipe(
      tap(() => {
        this.notifications.success(successMessage);
      })
    );
  }

  private createNewVersion(versionData: Version): Observable<any> {
    return this.api.plans.newPlanVersion(versionData).pipe(
      tap(() => {
        this.notifications.success(this.saveAsNewSuccessMessage);
      }),
      catchError(() => {
        this.notifications.error(this.saveAsNewErrorMessage);
        return EMPTY;
      })
    );
  }

  private saveVersionFusion(resp: ThreeDCMiddlewareResponse, newVersion: boolean): void {
    const formData = this.generateFormData(newVersion ? 0 : this.middlewareResponse.planVersionId);

    const subscription = this.api.plans.saveVersion(resp.planId, formData)
      .pipe(finalize(() => this.dialogRef.close()))
      .subscribe({
        next: () => {
          this.notifications.success(newVersion ? this.saveAsNewSuccessMessage : this.overwriteSuccessMessage);
          this.communication.notifyReloadViewData();
        },
        error: () => {
          this.notifications.error(newVersion ? this.saveAsNewErrorMessage : this.overwriteErrorMessage);
        }
      });

    this.entitySubscriptions.push(subscription);
  }

  private generateFormData(versionId: number): FormData {
    const resp = this.middlewareResponse as FusionMiddlewareResponse;
    const romfileName = this.isNewPlan ?
      resp.planId + '_' + resp.catalogType + '.Rom' :
      resp.romFileInfo.fileName;

    const romfile = new File(
      [resp.romFileInfo.romByteArray.buffer as ArrayBuffer],
      romfileName,
      {type: this.isNewPlan ? 'application/octet-stream' : resp.romFileInfo.type}
    );

    const previewFile = new File(
      [resp.preview.previewByteArray.buffer as ArrayBuffer],
      resp.preview.fileName,
      {type: resp.preview.type}
    );

    const formData = new FormData();
    formData.append('romFile', romfile, romfileName);
    formData.append('previewFile', previewFile, resp.preview.fileName);
    formData.append('model', JSON.stringify({
      id: versionId,
      versionNotes: this.entityForm.controls['versionNotes'].value,
      quoteOrderNumber: this.entityForm.controls['quoteOrderNumber'].value,
      catalogCode: resp.mainUniqueId,
      range: resp.mainRange,
      romItems: resp.lineItems
    }));

    return formData;
  }

  private initializeFormFields() {
    const resp = this.middlewareResponse as FusionMiddlewareResponse;
    const versionNotesControl = this.entityForm.controls['versionNotes'];
    versionNotesControl.setValue(resp.versionNotes ? resp.versionNotes : '');
    const quoteOrderNumberControl = this.entityForm.controls['quoteOrderNumber'];
    quoteOrderNumberControl.setValue(resp.quoteOrderNumber ? resp.quoteOrderNumber : '');
  }

  private getTranslationStrings() {
    const translateSubscription = this.translate.get([
      'dialog.newPlanSaved',
      'dialog.newTemplateSaved',
      'dialog.versionModified',
      'button.discardChanges',
      'button.discardPlan',
      'button.discardTemplate',
      'button.newVersion',
      'button.saveNewPlan',
      'button.saveNewTemplate',
      'dialog.overwritePlanSuccess',
      'dialog.overwritePlanError',
      'dialog.saveNewVersionSuccess',
      'dialog.saveNewVersionError',
      'builder.unassigned'
    ], {
      planId: this.middlewareResponse.planId,
      versionNumber: this.planVersionNumber
    }).subscribe((translations) => {
      this.newPlanTitleLabel = translations['dialog.newPlanSaved'];
      this.newTemplateTitleLabel = translations['dialog.newTemplateSaved'];
      this.versionModifiedTitleLabel = translations['dialog.versionModified'];
      this.discardPlanButtonText = translations['button.discardPlan'];
      this.discardTemplateButtonText = translations['button.discardTemplate'];
      this.discardChangesButtonText = translations['button.discardChanges'];
      this.newVersionButtonText = translations['button.newVersion'];
      this.newPlanButtonText = translations['button.saveNewPlan'];
      this.newTemplateButtonText = translations['button.saveNewTemplate'];
      this.overwriteSuccessMessage = translations['dialog.overwritePlanSuccess'];
      this.overwriteErrorMessage = translations['dialog.overwritePlanError'];
      this.saveAsNewSuccessMessage = translations['dialog.saveNewVersionSuccess'];
      this.saveAsNewErrorMessage = translations['dialog.saveNewVersionError'];
      this.unassignedBuilder = translations['builder.unassigned'];
    });
    this.entitySubscriptions.push(translateSubscription);
  }

}

