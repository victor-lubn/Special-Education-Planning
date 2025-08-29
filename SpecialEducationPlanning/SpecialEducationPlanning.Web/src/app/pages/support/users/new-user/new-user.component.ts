import { Component, ViewChild, OnInit } from '@angular/core';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { ApiService } from '../../../../core/api/api.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { ComponentCanDeactivate } from '../../../../shared/guards/pending-changes.guard';
import { BaseComponent } from '../../../../shared/base-classes/base-component';
import { User } from '../../../../shared/models/user';
import { AppEntitiesEnum } from '../../../../shared/models/app-enums';
import {
  UserCreateAndDetailsFormComponent
} from '../../../../shared/components/molecules/forms/user-create-and-details-form/user-create-and-details-form.component';


@Component({
  selector: 'tdp-new-user',
  templateUrl: 'new-user.component.html',
  styleUrls: ['new-user.component.scss']
})
export class NewUserComponent extends BaseComponent implements OnInit, ComponentCanDeactivate {

  @ViewChild(UserCreateAndDetailsFormComponent, { static: true }) userCreateAndDetailsFormComponent: UserCreateAndDetailsFormComponent;

  // Translation strings
  private notificationUserHeader = '';
  private notificationUserSuccess = '';
  private notificationUserError = '';

  constructor(
    private api: ApiService,
    private dialogs: DialogsService,
    private notifications: NotificationsService,
    private translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
  }

  hasChanges(): boolean {
    return !this.userCreateAndDetailsFormComponent.entityForm.pristine;
  }

  public userSubmittedHandler(user: User): void {
    const newUserSubscription = this.api.users.createUser(user, user.roleId)
      .subscribe((response) => {
        this.notifications.success(this.notificationUserHeader, this.notificationUserSuccess);
        this.userCreateAndDetailsFormComponent.cancelEdit(response);
        this.navigateTo('/support/users/' + response.id);
      }, (error) => {
        this.notifications.error(this.notificationUserHeader, this.notificationUserError);
      });
    this.entitySubscriptions.push(newUserSubscription);
  }

  saveNewUser() {
    this.userCreateAndDetailsFormComponent.submitForm();
  }

  protected initializeTranslationStrings() {
    const subscription = this.translate.get([
      'user.notification.header',
      'user.notification.createSuccess',
      'user.notification.createError'
    ]).subscribe((translations) => {
      this.notificationUserHeader = translations['user.notification.header'];
      this.notificationUserSuccess = translations['user.notification.createSuccess'];
      this.notificationUserError = translations['user.notification.createError'];
    });
    this.entitySubscriptions.push(subscription);
  }

  cancel() {
    this.navigateTo('/support/users/');
  }

  uploadCsv() {
    this.dialogs.uploadCSVFile(AppEntitiesEnum.user)
  }
}
