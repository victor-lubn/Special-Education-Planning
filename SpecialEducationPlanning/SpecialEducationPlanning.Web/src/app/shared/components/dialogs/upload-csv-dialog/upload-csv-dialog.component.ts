import { Component, ViewChild, ElementRef, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { BaseEntity } from '../../../base-classes/base-entity';
import { ApiService } from '../../../../core/api/api.service';
import { SimpleInformationDialogComponent } from '../simple-information-dialog/simple-information-dialog.component';

@Component({
  selector: 'tdp-upload-csv-dialog',
  templateUrl: 'upload-csv-dialog.component.html',
  styleUrls: ['upload-csv-dialog.component.scss']
})
export class UploadCSVDialogComponent extends BaseEntity {

  public file: File;
  public csvUploadSuccess: boolean;
  public entitiesUploaded: number;
  public uploadErrors: string[];

  private maxFilesAllowed = 1;

  @ViewChild('uploadExplorer')
  inputButton: ElementRef;

  constructor(
    private api: ApiService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<UploadCSVDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { entity: string }
  ) {
    super();
    this.dialogRef.disableClose = true;
    this.file = null;
    this.csvUploadSuccess = false;
    this.entitiesUploaded = 0;
    this.uploadErrors = [];
  }

  public onClickSpan(): void {
    this.inputButton.nativeElement.click();
  }

  public onFilesChange(event: FileList): void {
    if (event.length > this.maxFilesAllowed) {
      this.openErrorDialog('dialog.uploadCSV.title', 'dialog.uploadDialogMaxFilesError');
    } else {
      const newFile = event.item(0);
      if (newFile.name.includes('.csv')) {
        this.file = newFile;
      } else {
        this.openErrorDialog('dialog.uploadCSV.title', 'dialog.uploadCSV.wrongTypeErrorMsg');
      }
      this.inputButton.nativeElement.value = '';
    }
  }

  public cancelDialog(): void {
    if (this.file) {
      const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent,
        {
          width: '400px',
          data: {
            titleStringKey: 'dialog.uploadCSV.title',
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

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public submitFiles(): void {
    const subscription = this.api.csv.uploadCSV(this.data.entity, this.generateFormData(this.file))
      .subscribe((response: number) => {
        this.entitiesUploaded = response;
        this.csvUploadSuccess = true;
        this.uploadErrors = [];
      },
      (errorResponse) => {
        this.entitiesUploaded = 0;
        this.csvUploadSuccess = false;
        this.uploadErrors = errorResponse.error;
      });
    this.entitySubscriptions.push(subscription);
  }

  public deleteFile(): void {
    this.file = null;
    this.entitiesUploaded = 0;
    this.csvUploadSuccess = false;
    this.uploadErrors = [];
  }

  private generateFormData(file: File): FormData {
    const formData = new FormData();
    formData.append('csvFile', file, file.name);
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
