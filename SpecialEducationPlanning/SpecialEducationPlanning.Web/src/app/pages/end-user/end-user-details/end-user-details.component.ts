import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { EndUserFormComponent } from '../../../shared/components/forms/end-user-form/end-user-form.component';
import { ApiService } from '../../../core/api/api.service';
import { EndUser } from '../../../shared/models/end-user';
import { BaseComponent } from '../../../shared/base-classes/base-component';

import { NotificationsService } from 'angular2-notifications';
import { Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-end-user-details',
  templateUrl: './end-user-details.component.html',
  styleUrls: ['./end-user-details.component.scss']
})
export class EndUserDetailsComponent extends BaseComponent implements OnInit {

  public endUserValidation: any;

  public endUser: EndUser;
  protected endUserId: number;

  //Translation strings
  protected endUserInfo = '';
  protected endUserUpdatedSuccessMsg = '';
  protected endUserUpdatedErrorMsg = '';

  @ViewChild(EndUserFormComponent)
  private endUserForm: EndUserFormComponent;
  
  constructor(
    protected api: ApiService,
    protected notification: NotificationsService,
    protected activatedRoute: ActivatedRoute,
    protected translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
    const routerSubscription = this.activatedRoute.params.subscribe(params => {
      this.endUserId = +params["id"];
      this.getEndUser(this.endUserId);
    });
    this.entitySubscriptions.push(routerSubscription);
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

  public endUserSubmitHandler(id: number, endUser: EndUser) {
    const subscription = this.api.endUsers
      .putEndUser(id, endUser)
      .subscribe(
        (response) => {
          this.notification.success(this.endUserInfo, this.endUserUpdatedSuccessMsg);
          this.endUserForm.cancelEdit(response);
          this.endUser = response;
        },
        (error) => {
          this.notification.error(this.endUserUpdatedErrorMsg);
        }
      );
    this.entitySubscriptions.push(subscription);
  }

  public getEndUser(id: number) {
    const subscription = this.api.endUsers
      .getEndUser(id)
      .subscribe(response => {
        this.endUser = response;
      });
    this.entitySubscriptions.push(subscription);
  }

  protected initializeTranslationStrings() {
    const translateSubscription = this.translate.get([
      'notification.endUserInfo',
      'notification.endUserUpdatedSuccess',
      'notification.endUserUpdatedError'
    ]).subscribe((translations) => {
        this.endUserInfo = translations['notification.endUserInfo'];
        this.endUserUpdatedSuccessMsg = translations['notification.endUserUpdatedSuccess'];
        this.endUserUpdatedErrorMsg = translations['notification.endUserUpdatedError'];
      });
      this.entitySubscriptions.push(translateSubscription);
    }
}
