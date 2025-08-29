import { Component } from '@angular/core';
import { MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: 'tdp-publish-plan-error-dialog',
  templateUrl: './publish-plan-error-dialog.component.html'
})
export class PublishPlanErrorDialogComponent {

  constructor(
    private _dialogRef: MatDialogRef<PublishPlanErrorDialogComponent>,
  ) {
  }

  closeDialog() {
    this._dialogRef.close();
  }
}
