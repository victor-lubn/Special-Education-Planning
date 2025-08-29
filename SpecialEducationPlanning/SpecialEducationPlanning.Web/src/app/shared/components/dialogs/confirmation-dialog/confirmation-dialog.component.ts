import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface ConfirmationDialogData {
  titleStringKey: string;
  messageStringKey: string;
  cancelationStringKey?: string;
  confirmationStringKey?: string;
  width?: string;
  height?: string;
}

@Component({
  selector: 'tdp-confirmation-dialog',
  templateUrl: 'confirmation-dialog.component.html',
  styleUrls: ['confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent implements OnInit {

  public width: string = '600px';
  public height: string = '300px';
  public titleStringKey: string;
  public messageStringKey: string;

  public cancelationStringKey: string;
  public confirmationStringKey: string;

  constructor(
    private _dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: ConfirmationDialogData,
  ) {
    this.titleStringKey = '';
    this.messageStringKey = '';
    this.cancelationStringKey = 'booleanResponse.no';
    this.confirmationStringKey = 'booleanResponse.yes';
    this._dialogRef.disableClose = true;
  }

  ngOnInit(): void {
    this.titleStringKey = this._data.titleStringKey;
    this.messageStringKey = this._data.messageStringKey;
    if (this._data.cancelationStringKey) {
      this.cancelationStringKey = this._data.cancelationStringKey;
    }
    if (this._data.confirmationStringKey) {
      this.confirmationStringKey = this._data.confirmationStringKey;
    }
    if (this._data.width) {
      this.width = this._data.width;
    }
    if (this._data.height) {
      this.height = this._data.height;
    }
  }

  public onConfirm(): void {
    this._dialogRef.close(true);
  }

  public onCancel(): void {
    this._dialogRef.close(false);
  }

}
