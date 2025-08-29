import { Component, OnInit } from '@angular/core';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { BaseComponent } from '../../../shared/base-classes/base-component';

@Component({
  selector: 'tdp-login-offline',
  templateUrl: './login-offline.component.html',
  styleUrls: ['./login-offline.component.scss']
})
export class LoginOfflineComponent extends BaseComponent implements OnInit {

  constructor(
    private dialogs: DialogsService
  ) {
    super();
  }

  ngOnInit(): void {
    this.workOffline();
  }

  public workOffline(): void {
    const data = {
      title: 'dialog.workOffline.title',
      description: 'dialog.workOffline.description',
      button: 'dialog.workOffline.action',
    }
    this.dialogs.offlineSimpleDialog(
      data
    ).then(() => {
      this.navigateTo('/offline/planOffline/');
    });
  }

}
