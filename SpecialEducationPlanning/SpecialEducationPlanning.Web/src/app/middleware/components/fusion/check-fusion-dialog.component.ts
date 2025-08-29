import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { UserInfoService } from '../../../core/services/user-info/user-info.service';
import { FusionAcl } from './fusion-acl';
import { CommunicationService } from '../../../core/services/communication/communication.service';

@Component({
  selector: 'tdp-fusion-status-dialog',
  templateUrl: 'check-fusion-dialog.component.html',
})
export class MiddlewareFusionStatusDialogComponent {

  constructor(
    private dialogRef: MatDialogRef<MiddlewareFusionStatusDialogComponent>,
    private userInfoService: UserInfoService,
    private communication: CommunicationService
  ) {
    this.dialogRef.disableClose = true;
  }

  close() {
    this.userInfoService.removePermission(FusionAcl);
    this.communication.notifyPermissionsUpdated();
    this.dialogRef.close();
  }
}
