import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'tdp-connection-issue-dialog',
  templateUrl: 'connection-issue-dialog.component.html',
  styleUrls: ['connection-issue-dialog.component.scss']
})
export class ConnectionIssueDialogComponent {

  constructor(
    private _dialogRef: MatDialogRef<ConnectionIssueDialogComponent>
  ) {
    this._dialogRef.disableClose = true;
  }

  public retry(): void {
    this._dialogRef.close(false);
  }

  public acceptAction(): void {
    this._dialogRef.close(true);
  }

}
