import { Component, OnInit, Inject, NgZone, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

import { TranslateService } from '@ngx-translate/core';

import { InformationDialogComponent } from '../information-dialog/information-dialog.component';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { PlanOffline, VersionOffline } from '../../../models/plan-offline';
import { DocumentModel } from '../../../../middleware/models/document.model';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { iconNames } from '../../atoms/icon/icon.component';
import { BaseComponent } from '../../../base-classes/base-component';
import { PlanPreviewComponentData } from '../plan-preview/plan-preview.model';

@Component({
  selector: 'tdp-plan-preview-offline-dialog',
  templateUrl: 'plan-preview-offline-dialog.component.html',
  styleUrls: ['plan-preview-offline-dialog.component.scss']
})
export class PlanPreviewOfflineDialogComponent extends BaseComponent implements OnInit {

  public imageUrl: SafeUrl;
  public loadingImage: boolean;

  private imageBuffer: ArrayBuffer;

  public currentOfflinePlan: PlanOffline;
  public currentOfflineVersion: VersionOffline;
  public showViewDetailsButton;

  // Translation strings
  public previewUnavailableTitle: string = '';
  public previewUnavailableMessage: string = '';
  public unableToPrintPreviewTitle: string = '';
  public unableToPrintPreviewMessage: string = '';
  public accept: string = '';
  public cancel: string = '';

  constructor(
    private dialogRef: MatDialogRef<PlanPreviewOfflineDialogComponent>,
    private sanitizer: DomSanitizer,
    private dialog: MatDialog,
    private offlineMiddlewareService: OfflineMiddlewareService,
    private fusionMiddleware: EducationToolMiddlewareService,
    private ngZone: NgZone,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: { version: VersionOffline, plan: PlanOffline, showButton: PlanPreviewComponentData },
    private cdref: ChangeDetectorRef
  ) {
    super();
    this.currentOfflinePlan = this.data.plan;
    this.currentOfflineVersion = this.data.version;
    this.loadingImage = true;
  }

  ngOnInit(): void {
    this.showViewDetailsButton = this.data.showButton;
    this.cdref.detectChanges();
    this.getTranslationStrings();
    this.getPreview(this.currentOfflineVersion.previewPath);
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public printPreview(): void {
    if (this.imageBuffer.byteLength) {
      window.print();
    }
  }

  public getPreview(previewPath: string): void {
    const getPreviewSubscription = this.offlineMiddlewareService.getPreviewFileObservable(previewPath)
      .subscribe((data) => {
        if (data) {
          this.loadingImage = false;
          this.imageBuffer = data;
          const imageB64 = this.bufferToBase64(this.imageBuffer);
          const url = 'data:image/jpeg;base64,' + imageB64;
          this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(url);
        } else {
          this.loadingImage = false;
          this.closeDialog();
          this.ngZone.run(() => {
            const dialog = this.dialog.open(InformationDialogComponent, {
              width: '32em',
              data: {
                titleStringKey: this.previewUnavailableTitle,
                messageStringKey: this.previewUnavailableMessage,
                htmlText: false,
                cancel: this.cancel,
                accept: this.accept,
                image: iconNames.size48px.PREVIEW_UNAVAILABLE
              }
            });
          });
        }
      });
    this.entitySubscriptions.push(getPreviewSubscription);
  }

  public goToPlanDetails() {
    this.navigateTo('/offline/planOffline/' + this.currentOfflinePlan.id_offline);
    this.closeDialog();
  }

  openInFusion() {
    this.goToPlanDetails();
    const getRomFileSubscription = this.offlineMiddlewareService.getRomFileObservable(this.currentOfflineVersion.romPath)
    .subscribe((results) => {
      if (results) {
        const romFile = {
          type: 'application/octet-stream',
          fileName: this.currentOfflinePlan.planNumber,
          romByteArray: results
        };
        const model = this.generateFusionModel(romFile);
        this.fusionMiddleware.openDocument(model);
        this.closeDialog();
      } else {
        this.closeDialog();
        this.ngZone.run(() => {
          const dialog = this.dialog.open(InformationDialogComponent, {
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

  bufferToBase64(buf): string {
    const binstr = Array.prototype.map.call(buf, function (ch) {
        return String.fromCharCode(ch);
    }).join('');
    return btoa(binstr);
  }

  private generateFusionModel(romFile: any) {
    return new DocumentModel(
      false,
      this.currentOfflinePlan.id_offline,
      this.currentOfflinePlan.planNumber,
      this.currentOfflineVersion.catalogueCode,
      null,
      null,
      this.currentOfflineVersion.id_offline,
      this.currentOfflineVersion.id_offline.toString(),
      romFile,
      this.currentOfflineVersion.versionNotes,
      this.currentOfflineVersion.quoteOrderNumber,
      null,
      this.currentOfflinePlan.planName,
      null,
      this.currentOfflinePlan.id_offline
    );
  }

  private getTranslationStrings(): void {
    const translateSubscription = this.translate.get([
      'dialog.previewUnavailable.title',
      'dialog.previewUnavailable.message',
      'dialog.unableToPrintPreview.title',
      'dialog.unableToPrintPreview.message',
      'button.cancel',
      'button.accept'
    ]).subscribe((translations) => {
        this.previewUnavailableTitle = translations['dialog.previewUnavailable.title'];
        this.previewUnavailableMessage = translations['dialog.previewUnavailable.message'];
        this.unableToPrintPreviewTitle = translations['dialog.unableToPrintPreview.title'];
        this.unableToPrintPreviewMessage = translations['dialog.unableToPrintPreview.message'];
        this.cancel = translations['button.cancel'];
        this.accept = translations['button.accept'];
      });
    this.entitySubscriptions.push(translateSubscription);
  }
}

