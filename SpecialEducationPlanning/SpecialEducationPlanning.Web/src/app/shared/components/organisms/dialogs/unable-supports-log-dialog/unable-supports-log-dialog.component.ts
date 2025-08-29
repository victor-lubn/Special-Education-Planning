import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';

@Component({
  selector: 'tdp-unable-supports-log-dialog',
  templateUrl: './unable-supports-log-dialog.component.html',
  styleUrls: ['./unable-supports-log-dialog.component.scss']
})
export class UnableSupportsLogDialogComponent extends BaseComponent{

  constructor(
    private _dialogRef: MatDialogRef<UnableSupportsLogDialogComponent>,
  ) {    super();
  }

  onCancel(): void {
    this._dialogRef.close({
    });
  }
}
