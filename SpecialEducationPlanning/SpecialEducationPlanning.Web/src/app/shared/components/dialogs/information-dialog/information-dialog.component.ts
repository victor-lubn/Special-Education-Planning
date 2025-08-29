import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'tdp-information-dialog',
  templateUrl: 'information-dialog.component.html',
  styleUrls: ['information-dialog.component.scss']
})
export class InformationDialogComponent implements OnInit {

  public title: string;
  public description: string;
  public cancel: string;
  public accept: string;
  public image?: string;
  public htmlText: boolean;

  constructor(
    private _dialogRef: MatDialogRef<InformationDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    private _data: { titleStringKey: string, messageStringKey: string, htmlText: boolean, cancel?: string, accept?: string, image?: string,}
  ) {
    this.title = '';
    this.description = '';
    this.cancel = '';
    this.accept = '';
    this.image = '';
    this.htmlText = false;
    this._dialogRef.disableClose = true;
  }

  ngOnInit(): void {
    this.title = this._data.titleStringKey;
    this.description = this._data.messageStringKey;
    this.cancel = this._data.cancel;
    this.accept = this._data.accept;
    this.image = this._data.image;
    this.htmlText = this._data.htmlText;
  }

  public closeDialog(): void {
    this._dialogRef.close();
  }

}
