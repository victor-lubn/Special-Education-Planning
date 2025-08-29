import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'tdp-three-dc-unavailable-dialog',
  templateUrl: './three-dc-unavailable-dialog.component.html'
})
export class ThreeDcUnavailableDialogComponent {

  constructor(private dialogRef: MatDialogRef<ThreeDcUnavailableDialogComponent>,) {
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }
}
