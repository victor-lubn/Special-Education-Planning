import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { SystemLog } from '../../../models/system-log';

@Component({
  selector: 'tdp-system-log-detail-dialog',
  templateUrl: 'system-log-detail-dialog.component.html',
  styleUrls: ['system-log-detail-dialog.component.scss']
})
export class SystemLogDetailDialogComponent implements OnInit {

  public systemlog: SystemLog;

  constructor(
    private dialogRef: MatDialogRef<SystemLogDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    private data: {
      logKeyLog: SystemLog,
      htmlText: boolean
    }
  ) {
    this.dialogRef.disableClose = true;
  }

  ngOnInit(): void {
    this.systemlog = this.data.logKeyLog;
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

}
