import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'tdp-back-online-dialog',
  templateUrl: 'back-online-dialog.component.html',
  styleUrls: ['back-online-dialog.component.scss']
})
export class BackOnlineDialogComponent {

  constructor(
    private _dialogRef: MatDialogRef<BackOnlineDialogComponent>
  ) {
    this._dialogRef.disableClose = true;
  }

  public cancelAndCloseDialog(): void {
    this._dialogRef.close(false);
  }

  public acceptAction(): void {
    this._dialogRef.close(true);
  }

}
