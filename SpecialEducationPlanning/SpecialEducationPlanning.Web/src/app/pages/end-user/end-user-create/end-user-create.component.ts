import { Component, ViewChild } from '@angular/core';

import { ApiService } from '../../../core/api/api.service';
import { BaseComponent } from '../../../shared/base-classes/base-component';
import { EndUserFormComponent } from '../../../shared/components/forms/end-user-form/end-user-form.component';
import { EndUser } from '../../../shared/models/end-user';

import { NotificationsService } from 'angular2-notifications';
import { Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-end-user-create.component',
  templateUrl: './end-user-create.component.html',
  styleUrls: ['./end-user-create.component.scss']
})

export class EndUserCreateComponent extends BaseComponent{
  
  public endUserValidation: any;

  //translate strings
  protected endUserInfo = '';
  protected endUserCreatedSuccessMsg = '';
  protected endUserCreatedErrorMsg = '';

  @ViewChild(EndUserFormComponent, { static: true })
  private endUserForm: EndUserFormComponent;

  constructor(
    protected api: ApiService,
    protected notification: NotificationsService,
    protected translate: TranslateService
  ) {
    super();
    this.initializeTranslationStrings();
  }

  getEndUserValidation(): any {
    let output = this.checkEndUserValidation();
    return output;
  }

  checkEndUserValidation(): any {
    return {
          titleId: [null],
          firstName: [null],
          surname: [null, Validators.required],
          contactEmail: [null],
          countryCode: [null],
          postcode: [null, [Validators.required]],
          address1: [null, Validators.required],
          address2: [null],
          address3: [null],
          address4: [null],
          address5: [null],
          mobileNumber: [null],
          landLineNumber: [null],
          comment: [null]
    };
  }

  hasChanges(): boolean {
    return this.endUserForm.entityForm.dirty;
  }

  public endUserSubmitHandler(endUser: EndUser) {
    const subscription = this.api.endUsers
      .postEndUser(endUser)
      .subscribe(
        response => {
          this.notification.success(
            this.endUserInfo,
            this.endUserCreatedSuccessMsg
          );
          this.endUserForm.cancelEdit(response);
          this.navigateTo('/enduser/' + response.id);
        },
        error => {
          this.notification.error(
            this.endUserCreatedErrorMsg
          );
        }
      );
    this.entitySubscriptions.push(subscription);
  }
  
  protected initializeTranslationStrings() {
    const translateSubscription = this.translate.get([
      'notification.endUserInfo',
      'notification.endUserCreatedSuccess',
      'notification.endUserCreatedError'
    ]).subscribe((translations) => {
        this.endUserInfo = translations['notification.endUserInfo'];
        this.endUserCreatedSuccessMsg = translations['notification.endUserCreatedSuccess'];
        this.endUserCreatedErrorMsg = translations['notification.endUserCreatedError'];
      });
      this.entitySubscriptions.push(translateSubscription);
    }
}