import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'tdp-error-message-dialog',
  templateUrl: 'error-message-dialog.component.html',
  styleUrls: ['./error-message-dialog.component.scss']
})
export class ErrorMessageDialogComponent implements OnInit {

  public title: string;
  public description: string;

  constructor(
    private _dialogRef: MatDialogRef<ErrorMessageDialogComponent>,
    @Inject(MAT_DIALOG_DATA) 
    private _data: {descriptionString: string, titleString: string }
    ) {
        this.title = '';
        this.description = '';
        this._dialogRef.disableClose = true;
    }
  ngOnInit(): void {
    this.title = this._data.titleString;
    this.description = this._data.descriptionString;
  }

  close() {
    this._dialogRef.close();
  }
}