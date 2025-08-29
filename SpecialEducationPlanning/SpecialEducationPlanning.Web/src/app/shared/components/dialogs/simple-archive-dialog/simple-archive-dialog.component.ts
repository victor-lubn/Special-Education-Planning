import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface SimpleArchiveDialogData {
  titleStringKey: string
}

@Component({
  selector: 'tdp-simple-archive-dialog',
  templateUrl: 'simple-archive-dialog.component.html',
  styleUrls: ['simple-archive-dialog.component.scss']
})
export class SimpleArchiveDialogComponent implements OnInit {

  public width: string = '400px';
  public height: string = '200px';
  public titleStringKey: string;
  public cancelationStringKey: string;
  public confirmationStringKey: string;


  constructor(
    private _dialogRef: MatDialogRef<SimpleArchiveDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: SimpleArchiveDialogComponent,
  ) {
    this.titleStringKey = '';
    this.cancelationStringKey = 'booleanResponse.no';
    this.confirmationStringKey = 'booleanResponse.yes';
    this._dialogRef.disableClose = true;
  }

  ngOnInit(): void {
    this.titleStringKey = this._data.titleStringKey;
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
