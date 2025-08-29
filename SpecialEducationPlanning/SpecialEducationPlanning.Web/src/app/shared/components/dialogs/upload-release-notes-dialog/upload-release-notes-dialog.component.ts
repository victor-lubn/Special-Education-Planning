import { Component, ViewChild, ElementRef, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Validators } from '@angular/forms';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { ApiService } from '../../../../core/api/api.service';
import { ReleaseInfo } from '../../../models/release-info';
import { FormComponent } from '../../../base-classes/form-component';
import { TDPVersionValidation } from '../../../validators/tdp-version.validator';
import { FusionVersionValidation } from '../../../validators/fusion-version.validator';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { SimpleInformationDialogComponent } from '../simple-information-dialog/simple-information-dialog.component';

@Component({
  selector: 'tdp-upload-release-notes-dialog',
  templateUrl: 'upload-release-notes-dialog.component.html',
  styleUrls: ['upload-release-notes-dialog.component.scss']
})
export class UploadReleaseNotesDialogComponent extends FormComponent implements OnInit {

  public readonly titleMaxLength: number = 150;
  public readonly maxFilesAllowed: number = 1;
  public file: File;
  public minDate: Date;
  private acceptedMimeType = 'application/pdf';
  public releaseInfoId: number;

  @ViewChild('uploadExplorer')
  inputButton: ElementRef;

  private documentUploadSuccess: string = '';
  private documentUploadError: string = '';
  private documentDuplicatedError: string = '';
  private isFileUploaded: boolean = false;

  constructor(
    private notifications: NotificationsService,
    private communication: CommunicationService,
    private api: ApiService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<UploadReleaseNotesDialogComponent>,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) private inputData: { releaseInfoId: number }
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      title: [null, [Validators.required, Validators.maxLength(this.titleMaxLength)]],
      dateTime: ['', [Validators.required]],
      version: [null, [Validators.required, TDPVersionValidation]],
      fusionVersion: [null, [FusionVersionValidation]]
    });
    this.file = null;
    this.dialogRef.disableClose = true;
    this.minDate = new Date();
    this.releaseInfoId = this.inputData.releaseInfoId;
  }

  ngOnInit(): void {
    let subscription: Subscription;
    if (this.releaseInfoId) {
      subscription = this.api.releaseInfo.getReleaseInfo(this.releaseInfoId)
        .subscribe((response: ReleaseInfo) => {
          this.entityForm.patchValue(response);
          this.entityForm.disable();
        });
      this.entitySubscriptions.push(subscription);
    }
    subscription = this.translate.get([
      'notification.documentUploadSuccess',
      'notification.documentUploadError',
      'notification.documentDuplicatedError'
    ]).subscribe((translations) => {
      this.documentUploadSuccess = translations['notification.documentUploadSuccess'];
      this.documentUploadError = translations['notification.documentUploadError'];
      this.documentDuplicatedError = translations['notification.documentDuplicatedError'];
    });
    this.entitySubscriptions.push(subscription);
  }

  public onClickSpan(): void {
    this.inputButton.nativeElement.click();
  }

  public onFileschange(event: FileList): void {
    if (event.length > this.maxFilesAllowed) {
      this.openErrorDialog('topbar.uploadReleaseNotesDialogMenu', 'dialog.uploadDialogMaxFilesError');
    } else {
      const newFile = event.item(0);
      if (newFile.type !== this.acceptedMimeType) {
        this.openErrorDialog('topbar.uploadReleaseNotesDialogMenu', 'dialog.uploadReleaseNotes.wrongTypeErrorMsg');
      } else {
        this.file = newFile;
        this.inputButton.nativeElement.value = '';
        this.isFileUploaded = true;
      }
    }
  }

  public cancelDialog(): void {
    if (this.file || this.entityForm.dirty) {
      const confirmationTitleStringKey = (this.releaseInfoId === 0 ? 'releaseNotesPage.upload' : 'topbar.uploadReleaseNotesDialogMenu');
      const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent,
        {
          width: '400px',
          data: {
            titleStringKey: confirmationTitleStringKey,
            messageStringKey: 'dialog.genericCancelDialog'
        }
      });
      const dialogSubscription = confirmationDialogRef.afterClosed()
        .subscribe((confirmation: boolean) => {
          if (confirmation) {
            this.dialogRef.close();
          }
        });
      this.entitySubscriptions.push(dialogSubscription);
    } else {
      this.dialogRef.close();
    }
  }

  public submitUpdate(): void {
    const fileFormData = this.generateFormData(this.file);
    const updateDocumentSubscription = this.api.releaseInfo.createReleaseInfoDocument(this.releaseInfoId, fileFormData)
      .subscribe((releaseResponse: ReleaseInfo) => {
        this.notifications.success(this.documentUploadSuccess);
        this.communication.notifyReloadViewData();
        this.dialogRef.close(releaseResponse);
      }, (error) => {
        this.notifications.error(this.documentDuplicatedError);
      });
    this.entitySubscriptions.push(updateDocumentSubscription);
  }

  public submitFiles(): void {
    const createReleaseInfoSubscription = this.api.releaseInfo.createReleaseInfo({
      ...this.entityForm.value,
      fusionVersion: this.entityForm.get('fusionVersion').value === '' ? null : this.entityForm.get('fusionVersion').value,
      dateTime: (this.entityForm.controls['dateTime'].value as Date).toISOString()
    })
      .subscribe((response: ReleaseInfo) => {
        const fileFormData = this.generateFormData(this.file);
        // TODO: replace with unified endpoint when it's done
        const createDocumentSubscription = this.api.releaseInfo.createReleaseInfoDocument(response.id, fileFormData)
          .subscribe((releaseResponse: ReleaseInfo) => {
            this.notifications.success(this.documentUploadSuccess);
            this.communication.notifyReloadViewData();
            this.dialogRef.close(releaseResponse);
          },
          (error) => {
            const deleteReleaseInfo = this.api.releaseInfo.deleteReleaseInfo(response.id)
              .subscribe(() => {
                this.notifications.error(this.documentUploadError);
              });
            this.entitySubscriptions.push(deleteReleaseInfo);
          });
        this.entitySubscriptions.push(createDocumentSubscription);
      }, (error) => {
        this.notifications.error(this.documentDuplicatedError);
      });
    this.entitySubscriptions.push(createReleaseInfoSubscription);
  }

  public deleteFile(): void {
    this.file = null;
    this.isFileUploaded = false;
  }

  private generateFormData(file: File): FormData {
    const formData = new FormData();
    formData.append('releaseNotes', file, file.name);
    return formData;
  }

  private openErrorDialog(title: string, message: string) {
    const dialog = this.dialog.open(SimpleInformationDialogComponent, {
      width: '400px',
      data: {
        titleStringKey: title,
        messageStringKey: message
      }
    });
  }
}
