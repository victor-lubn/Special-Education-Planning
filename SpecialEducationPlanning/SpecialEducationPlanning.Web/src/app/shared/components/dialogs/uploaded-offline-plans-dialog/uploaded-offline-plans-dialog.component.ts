import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SyncedPlan } from '../../../models/plan-offline';
export interface ConfirmationDialogData {
  titleStringKey: string;
  messageStringKey: string;
}

@Component({
  selector: 'tdp-uploaded-offline-plans-dialog',
  templateUrl: 'uploaded-offline-plans-dialog.component.html',
  styleUrls: ['uploaded-offline-plans-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UploadedOfflinePlansDialogComponent implements OnInit  {

  public syncedPlans: SyncedPlan [];
  public notSyncedPlans: number [];

  constructor(
    private dialogRef: MatDialogRef<UploadedOfflinePlansDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: {syncedPlans: SyncedPlan[], notSyncedPlans: number[]}
  ) {
    this.syncedPlans = [];
    this.notSyncedPlans = [];
  }

  ngOnInit(): void {
    this.syncedPlans = this._data.syncedPlans;
    this.notSyncedPlans = this._data.notSyncedPlans;
  }

  public cancelAndCloseDialog(): void {
    this.dialogRef.close(false);
  }

  public navigateToUnassignedPlans(): void {
    this.dialogRef.close(true);
  }

  public acceptAction(): void {
    this.dialogRef.close(false);
  }

}
