import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface SimpleInformationDialogData {
  titleStringKey: string,
  messageStringKey: string,
  messageStringKey2?: string
}

@Component({
  selector: 'tdp-simple-information-dialog',
  templateUrl: 'simple-information-dialog.component.html',
  styleUrls: ['simple-information-dialog.component.scss']
})
export class SimpleInformationDialogComponent implements OnInit {

  public titleStringKey: string;
  public messageStringKey: string;
  public messageStringKey2: string;
  public acceptStringKey: string;
  public width: string = '500px';
  public height: string = '300px';

  constructor(
    private _dialogRef: MatDialogRef<SimpleInformationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: SimpleInformationDialogData,
  ) {
    this.titleStringKey = '';
    this.messageStringKey = '';
    this.messageStringKey2 = '';
    this.acceptStringKey = 'button.accept';
  }

  ngOnInit(): void {
    this.titleStringKey = this._data.titleStringKey;
    this.messageStringKey = this._data.messageStringKey;
    if (this._data.messageStringKey2) {
      this.messageStringKey2 = this._data.messageStringKey2;
    }
  }

  public handleClose(): void {
    this._dialogRef.close();
  }

}
