import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ApiService } from '../../../../core/api/api.service';
import { BaseComponent } from '../../../base-classes/base-component';
import { finalize } from 'rxjs/operators';
import { Version } from '../../../models/version';
import { InformationDialogComponent } from '../information-dialog/information-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { iconNames } from '../../atoms/icon/icon.component';
import { PlanPreviewComponentData } from './plan-preview.model';
import { EducationToolService } from "../../../../core/Education-tool/Education-tool.service";

@Component({
  selector: 'tdp-plan-preview',
  templateUrl: 'plan-preview.component.html',
  styleUrls: ['plan-preview.component.scss']
})
export class PlanPreviewComponent extends BaseComponent implements OnInit {

  public imageUrl: SafeUrl;
  public loadingImage: boolean;

  private imageBlob: Blob;
  private imageBuffer: ArrayBuffer;

  public planId: string = '';
  public showViewDetailsButton: boolean;

  // Translation strings
  private previewUnavailableTitle = '';
  private previewUnavailableOnlineMessage = '';
  public previewTitle = '';
  public previewPromoteMaster = '';
  public previewPrint = '';
  public previewEdit = '';
  public previewView = '';
  public previewAltText = ''
  public cancel = '';
  public okay = '';

  constructor(
    private dialogRef: MatDialogRef<PlanPreviewComponent>,
    private sanitizer: DomSanitizer,
    private api: ApiService,
    private dialog: MatDialog,
    private translate: TranslateService,
    private EducationToolService: EducationToolService,
    @Inject(MAT_DIALOG_DATA) public data: PlanPreviewComponentData
  ) {
    super();
    this.loadingImage = true;
    this.imageBuffer = new ArrayBuffer(0);
  }

  ngOnInit(): void {
    this.planId = this.data.plan.planCode;
    this.showViewDetailsButton = this.data.showButton;
    this.initializeTranslationStrings();
    const versionSubscription = this.api.plans.getVersionById(this.data.versionId)
      .subscribe((version: Version) => {
        if (version.previewPath) {
          const previewSubscription = this.api.plans.getVersionPreview(this.data.versionId)
          .pipe(finalize(() => {
            this.loadingImage = false;
          }))
          .subscribe((response: ArrayBuffer) => {
            this.imageBuffer = response;
            this.imageBlob = new Blob([new Uint8Array(response)], { type: 'image/jpeg' });
            this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(this.imageBlob));
          });
          this.entitySubscriptions.push(previewSubscription);
        } else {
          const dialog = this.dialog.open(InformationDialogComponent, {
            width: '38em',
            data: {
              titleStringKey: this.previewUnavailableTitle,
              messageStringKey: this.previewUnavailableOnlineMessage,
              cancel: this.cancel,
              accept: this.okay,
              image: iconNames.size48px.PREVIEW_UNAVAILABLE,
              htmlText: true
            }
          });
          const dialogClosed = dialog.afterClosed()
            .subscribe(() => {
              this.dialogRef.close(false);
            });
          this.entitySubscriptions.push(dialogClosed);
        }
      });
    this.entitySubscriptions.push(versionSubscription);
  }

  public closeDialog(): void {
    this.dialogRef.close(false);
  }

  public printPreview(): void {
    if (this.imageBuffer.byteLength) {
      window.print();
    }
  }

  public openPlanInEducationTool() {
    const planData = {
      versionId: this.data.versionId,
      builderId: this.data.plan.builderId,
      catalogId: this.data.plan.catalogId,
      planId: this.data.plan.id,
      EducationOrigin : this.data.plan.EducationOrigin
    }
    this.goToPlanDetails();
    this.EducationToolService.getPlanDetailsAndOpenInEducationTool(planData);
    this.closeDialog();
  }

  public goToPlanDetails(): void {
      event.stopPropagation();
      this.closeDialog();
      this.navigateTo('/plan/' + this.data.plan.id);
  }

  public markMasterVersionHandler(): void {
    this.data.plan.masterVersionId = this.data.versionId;
     const subscription = this.api.plans.updatePlan(this.data.plan)
      .subscribe(
        (response) => {
          const newMasterVersion = this.getMasterVersion(this.data.versionId);
          const restOfVersions = this.data.planVersions.filter(version => version.id !== this.data.versionId);
          this.data.planVersions = [newMasterVersion, ...restOfVersions];
          this.dialogRef.close(true);
        },
        (error) => {
          //TODO: error notification when new notification service created
        }
      );
    this.entitySubscriptions.push(subscription);
  }

  public getMasterVersion(masterVersionId: number): Version {
    return this.data.planVersions.find(version => version.id === masterVersionId);
  }


  protected initializeTranslationStrings() {
    const translateSubscription = this.translate.get([
      'dialog.previewUnavailable.title',
      'dialog.previewUnavailable.messageOnline',
      'plan.planId',
      'button.promoteToMaster',
      'button.print',
      'button.edit',
      'button.viewPlan',
      'dialog.noPlanPreview',
      'button.cancel',
      'button.accept'
    ]).subscribe((translations) => {
      this.previewUnavailableTitle = translations['dialog.previewUnavailable.title'],
      this.previewUnavailableOnlineMessage = translations['dialog.previewUnavailable.messageOnline'];
      this.previewTitle = translations['plan.planId'];
      this.previewPromoteMaster = translations ['button.promoteToMaster']
      this.previewPrint = translations['button.print'];
      this.previewEdit = translations['button.edit'];
      this.previewView = translations['button.viewPlan'];
      this.previewAltText = translations['dialog.noPlanPreview'];
      this.cancel = translations['button.cancel'];
      this.okay = translations['button.accept'];
    })
    this.entitySubscriptions.push(translateSubscription);
  }
}

