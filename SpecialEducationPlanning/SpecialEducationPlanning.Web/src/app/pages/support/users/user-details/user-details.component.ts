import { ViewChild, OnInit, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { BaseComponent } from '../../../../shared/base-classes/base-component';
import { ComponentCanDeactivate } from '../../../../shared/guards/pending-changes.guard';
import { User } from '../../../../shared/models/user';
import { ApiService } from '../../../../core/api/api.service';
import {
  UserCreateAndDetailsFormComponent
} from '../../../../shared/components/molecules/forms/user-create-and-details-form/user-create-and-details-form.component';

@Component({
  selector: 'tdp-user-details',
  templateUrl: 'user-details.component.html',
  styleUrls: ['user-details.component.scss']
})
export class UserDetailsComponent extends BaseComponent implements ComponentCanDeactivate, OnInit {

  @ViewChild(UserCreateAndDetailsFormComponent, { static: true }) userCreateAndDetailsFormComponent: UserCreateAndDetailsFormComponent;

  public user: User;
  private userId: number;

  // Translation strings
  private notificationUserHeader = '';
  private notificationUserUpdateSuccess = '';
  private notificationUserUpdateError = '';

  constructor(
    private api: ApiService,
    private activatedRoute: ActivatedRoute,
    private notification: NotificationsService,
    private translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    const subscription = this.translate.get([
      'user.notification.header',
      'user.notification.updateSuccess',
      'user.notification.updateError'
    ])
      .subscribe((translations) => {
        this.notificationUserHeader = translations['user.notification.header'];
        this.notificationUserUpdateSuccess = translations['user.notification.updateSuccess'];
        this.notificationUserUpdateError = translations['user.notification.updateError'];
      });
    this.entitySubscriptions.push(subscription);
    const routerSubscription = this.activatedRoute.params
      .subscribe((params) => {
        this.userId = +params['id'];
        this.recoverViewData(this.userId);
      });
    this.entitySubscriptions.push(routerSubscription);
  }

  hasChanges() {
    return !this.userCreateAndDetailsFormComponent.entityForm.pristine;
  }

  public userSubmittedHandler(userSubmitted: User): void {
    const updateUserSubscription = this.api.users.updateUser(this.user.id, userSubmitted, userSubmitted.roleId)
      .subscribe((response) => {
        this.notification.success(this.notificationUserHeader, this.notificationUserUpdateSuccess);
        this.userCreateAndDetailsFormComponent.cancelEdit(response);
        this.user = response;
      }, (error) => {
        this.notification.error(this.notificationUserHeader, this.notificationUserUpdateError);
      });
    this.entitySubscriptions.push(updateUserSubscription);
  }

  private recoverViewData(userId: number): void {
    const userSubscription = this.api.users.getUserWithRoles(userId)
      .subscribe((response: User) => {
        this.user = {
          ...response,
          roleId: response.userRoles.length > 0 ? response.userRoles[0].role.id : null
        };
      });
    this.entitySubscriptions.push(userSubscription);
  }

  updateUser(): void {
    this.userCreateAndDetailsFormComponent.submitForm();
  }

  cancel() {
    this.navigateTo('/support/users/');
  }
}
