import {Component, OnDestroy, OnInit} from '@angular/core';

import { BaseComponent } from '../../../shared/base-classes/base-component';
import { UserInfoService } from '../../../core/services/user-info/user-info.service';
import { CommunicationService } from '../../../core/services/communication/communication.service';

@Component({
  selector: 'tdp-support-dashboard',
  templateUrl: 'support-dashboard.component.html',
  styleUrls: ['support-dashboard.component.scss']
})
export class SupportDashboardComponent extends BaseComponent implements OnInit, OnDestroy {
  route: string;
  hideUserBlock: boolean;
  hideAiepsBlock: boolean;
  hideReleaseNotesBlock: boolean;

  constructor(
    private userInfo: UserInfoService,
    private communication: CommunicationService
  ) {
    super();
    this.hideUserBlock = false;
    this.hideAiepsBlock = false;
    this.hideReleaseNotesBlock = false;
  }

  ngOnInit() {
    this.communication.notifyClearHomeFilters(false);
    this.communication.notifyAiepSelectorEnabled(false);
    this.hasPermissions();
    this.communication.subscribeToPermissionsUpdated(() => {
      this.hasPermissions();
    });
  }

  private hasPermissions() {
    this.hideUserBlock = !this.userInfo.hasPermission('User_Management');
    this.hideAiepsBlock = !this.userInfo.hasPermission('Structure_Management');
    this.hideReleaseNotesBlock = !this.userInfo.hasPermission('Data_Management');
  }

  ngOnDestroy() {
    super.ngOnDestroy();
  }
}

