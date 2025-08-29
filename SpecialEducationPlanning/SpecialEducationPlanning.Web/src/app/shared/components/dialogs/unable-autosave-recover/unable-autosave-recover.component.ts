import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'tdp-unable-autosave-recover',
  templateUrl: './unable-autosave-recover.component.html',
  styleUrls: ['./unable-autosave-recover.component.scss']
})
export class UnableAutosaveRecoverDialogComponent {

  constructor(
    private dialogRef: MatDialogRef<UnableAutosaveRecoverDialogComponent>,
  ){}

  closeDialog() {
    this.dialogRef.close();
  }
}
