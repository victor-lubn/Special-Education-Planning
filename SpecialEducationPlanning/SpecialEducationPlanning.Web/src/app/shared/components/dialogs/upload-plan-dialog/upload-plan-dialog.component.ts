import { Component, Inject, OnInit, ViewChild, ElementRef, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Validators } from '@angular/forms';

import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';

import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { SimpleInformationDialogComponent } from '../simple-information-dialog/simple-information-dialog.component'
import { ApiService } from '../../../../core/api/api.service';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { iconNames } from '../../atoms/icon/icon.component';
import { FormComponent } from '../../../base-classes/form-component';

@Component({
  selector: 'tdp-upload-plan-dialog',
  templateUrl: 'upload-plan-dialog.component.html',
  styleUrls: ['upload-plan-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UploadPlanDialogComponent extends FormComponent implements OnInit {

  private readonly typesAllowed: string[] = ['.rom', '.json', '.jpeg', '.jpg', '.png'];
  public uploadExtensionsString: string;
  public maxFilesAllowed: number;
  public maxFileSizeAllowed: number;
  public files: File[];

  public maxVersionNotesLength: number = 500;

  public cloudUploadFileIcon: string = iconNames.size48px.CLOUD_UPLOAD;
  public closeIcon: string = iconNames.size24px.CLEAR;

  @ViewChild('uploadExplorer', { static: true })
  inputButton: ElementRef;

  // Translation strings
  private uploadFilesDialogtitle: string;
  private errorDialogMsgMaxFiles: string;
  private errorDialogMsgMaxSizeFiles: string;
  private errorDialogMsgWrongTypes: string;
  private versionUploadSuccess: string;
  private versionUploadError: string;

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<UploadPlanDialogComponent>,
    private api: ApiService,
    private notifications: NotificationsService,
    private communication: CommunicationService,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) private data: {
      isNewVersion: boolean,
      versionId: number,
      planId: number
    }
  ) {
    super();
    this.dialogRef.disableClose = true;
    this.uploadExtensionsString = this.generateExtensionString(this.typesAllowed);
    this.maxFilesAllowed = 3;
    this.maxFileSizeAllowed = 10485760; // 10MB || = 12582912; // 12MB
    this.files = [];
    this.uploadFilesDialogtitle = '';
    this.errorDialogMsgMaxFiles = '';
    this.errorDialogMsgMaxSizeFiles = '';
    this.errorDialogMsgWrongTypes = '';
  }

  ngOnInit(): void {
    this.getTranslationStrings();
    this.initializeForm();
  }

  public initializeForm() {
    this.entityForm = this.formBuilder.group({
      uploadVersionNotes: [null, Validators.maxLength(this.maxVersionNotesLength)]
    })
  }

  public onClickSpan(): void {
    this.inputButton.nativeElement.click();
  }

  public onFileschange(event: FileList): void {
    const newFiles = Object.keys(event).map((property) => {
      return event.item(+property);
    });

    if (this.files.length + newFiles.length > this.maxFilesAllowed) {
      this.openErrorDialog(this.uploadFilesDialogtitle, this.errorDialogMsgMaxFiles);
    } else if (newFiles.find(f => f.size > this.maxFileSizeAllowed)) {
      this.openErrorDialog(this.uploadFilesDialogtitle, this.errorDialogMsgMaxSizeFiles);
    } else {
      this.files = [...this.files, ...newFiles];
      this.inputButton.nativeElement.value = '';
    }
  }

  public cancelDialog(): void {
    if (this.files.length || this.entityForm.dirty) {
      const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent,
        {
          width: '400px',
          data: {
            titleStringKey: 'dialog.uploadPlans',
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

  public deleteFile(file: File): void {
    this.files = this.files.filter((fileItem) => {
      return fileItem.name !== file.name;
    });
  }

  public submitFiles(): void {
    const map = this.getFileTypesMap();
    if (this.validateFileInput(map)) {
      this.generateFormData(this.data.versionId)
        .then((formData: FormData) => {
          const subscription = this.api.plans.saveVersion(this.data.planId, formData)
            .subscribe(() => {
              this.notifications.success(this.versionUploadSuccess);
              this.communication.notifyReloadViewData();
              this.dialogRef.close();
            }, (error) => {
              this.notifications.error(this.versionUploadError);
            });
          this.entitySubscriptions.push(subscription);
        });
    } else {
      this.openErrorDialog(this.uploadFilesDialogtitle, this.errorDialogMsgWrongTypes);
    }
  }

  private generateFormData(versionId: number): Promise<FormData> {
    return new Promise((resolve) => {

      const formData = new FormData();
      let file: File;
      let jsonFile: File;
      let filename: string;
      let filetype: string;

      for (let i = 0; i < this.files.length; i++) {
        file = this.files[i];
        filename = file.name;
        if (this.getExtension(filename) === '.rom') {
          filetype = 'romFile';
          formData.append(filetype, file, filename);
        } else if (['.jpeg', '.jpg', '.png'].includes(this.getExtension(filename))) {
          filetype = 'previewFile';
          formData.append(filetype, file, filename);
        } else {
          jsonFile = file;
        }
      }

      if (jsonFile) {
        const reader = new FileReader();
        reader.onload = () => {
          const jsonData = JSON.parse(reader.result as string);
          formData.append('model', JSON.stringify({
            id: versionId,
            versionNotes: this.entityForm.controls['uploadVersionNotes'].value,
            catalogCode: jsonData.MainUniqueId,
            range: jsonData.MainRange,
            romItems: jsonData.LineItems
          }));
          resolve(formData);
        };
        reader.readAsText(jsonFile);
      } else {
        formData.append('model', JSON.stringify({
          id: versionId,
          versionNotes: this.entityForm.controls['uploadVersionNotes'].value
        }));
        resolve(formData);
      }

    });
  }

  /**
   * Generates a single string to be used in the attribute Accept of Input
   * @param extensionList
   */
  private generateExtensionString(extensionList: string[]): string {
    let res = '';
    for (let i = 0; i < extensionList.length; i++) {
      res += extensionList[i];
      if (i !== extensionList.length - 1) {
        res += ', ';
      }
    }
    return res;
  }

  /**
   * Key: extension with dot, i.e.: '.rom'
   * Value: number of ocurrences of that extension
   * To count all images formats, .png and .jpg are counted as .jpeg in this map.
   */
  private getFileTypesMap(): Map<string, number> {
    const map = new Map<string, number>();
    let count: number;
    let type: string;
    for (let i = 0; i < this.files.length; i++) {
      type = this.getExtension(this.files[i].name);
      if (type === '.jpg' || type === '.png') {
        type = '.jpeg';
      }
      count = map.has(type) ? map.get(type) : 0;
      map.set(type, count + 1);
    }
    return map;
  }

  private getExtension(fileName: string): string {
    return fileName.match(new RegExp('\\.[0-9a-z]+$', 'i'))[0].toLowerCase();
  }

  private checkMandatoryFile(map: Map<string, number>, type: string): boolean {
    return map.get(type) === 1;
  }

  private checkAllowedTypes(map: Map<string, number>): boolean {
    for (const typeUploaded of Array.from(map.keys())) {
      if (!(this.typesAllowed).includes(typeUploaded)) {
        return false;
      }
    }
    return true;
  }

  private checkOptionalTypes(map: Map<string, number>, type: string): boolean {
    return (map.has(type) ? map.get(type) : 0) <= 1;
  }

  private validateFileInput(map: Map<string, number>): boolean {
    return this.checkMandatoryFile(map, '.rom') &&
      this.checkMandatoryFile(map, '.json') &&
      this.checkAllowedTypes(map) &&
      this.checkOptionalTypes(map, '.jpeg');
  }

  private openErrorDialog(title: string, message: string) {
    const dialog = this.dialog.open(SimpleInformationDialogComponent, {
      data: {
        titleStringKey: title,
        messageStringKey: message,
      }
    });
  }

  private getTranslationStrings() {
    const subscription = this.translate.get([
      'dialog.uploadPlans',
      'dialog.uploadDialogMaxFilesError',
      'dialog.uploadDialogMaxSizeError',
      'dialog.uploadDialogWrongTypesError',
      'notification.versionUploadSuccess',
      'notification.versionUploadError'
    ]).subscribe((translations) => {
      this.uploadFilesDialogtitle = translations['dialog.uploadPlans'];
      this.errorDialogMsgMaxFiles = translations['dialog.uploadDialogMaxFilesError'];
      this.errorDialogMsgMaxSizeFiles = translations['dialog.uploadDialogMaxSizeError'];
      this.errorDialogMsgWrongTypes = translations['dialog.uploadDialogWrongTypesError'];
      this.versionUploadSuccess = translations['notification.versionUploadSuccess'];
      this.versionUploadError = translations['notification.versionUploadError'];
    });
    this.entitySubscriptions.push(subscription);
  }

}
