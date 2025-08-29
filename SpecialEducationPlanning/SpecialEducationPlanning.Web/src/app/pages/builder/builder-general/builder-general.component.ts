import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';

import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';

import { FormComponent } from '../../../shared/base-classes/form-component';
import { ApiService } from '../../../core/api/api.service';
import { Builder } from '../../../shared/models/builder';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { BuilderFormComponent } from '../../../shared/components/forms/builder-form/builder-form.component';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { Validators } from '@angular/forms';

@Component({
  selector: 'tdp-builder-general',
  template: `<div></div>`
})
export class BuilderGeneralComponent extends FormComponent implements OnInit {

  public validation: UntypedFormGroup;
  public builder: Builder;

  // Translation strings
  protected changesSavedTitle = '';
  protected changesSavedMessage = '';
  protected saveErrorTitle = '';
  protected saveErrorMessage = '';
  protected invalidFormTitle = '';
  protected invalidFormMessage = '';
  protected builderDuplicateValidationError = '';
  protected builderCreateErrorMsg = '';
  protected planInfoMessage = '';
  protected planCreatedSuccessMsg = '';
  protected planCreatedErrorMsg = '';

  public functionName: string;

  @ViewChild(BuilderFormComponent)
  protected builderForm: BuilderFormComponent;

  protected readonly builderNumberMaxLength: number = 30;
  protected readonly builderNotesMaxLength: number = 500;

  constructor(
    public api: ApiService,
    public notifications: NotificationsService,
    public dialogs: DialogsService,
    public translate: TranslateService,
    public communications: CommunicationService,
  ) {
    super();
    this.checkValidation();
  }

  ngOnInit(): void {
    this.communications.notifyClearHomeFilters(false);
    this.communications.notifyAiepSelectorEnabled(false);
    this.initializeTranslationStrings();
  }

  public checkValidation(): void {
    this.validation = this.formBuilder.group({
      id: [0],
      accountNumber: [{ value: null, disabled: true }],
      tradingName: [null, Validators.required],
      name: [null],
      country: [null],
      postcode: ['', [Validators.required]],
      address1: [null, Validators.required],
      address2: [null],
      address3: [null],
      mobileNumber: [null, Validators.maxLength(this.builderNumberMaxLength)],
      email: [null, [Validators.email]],
      landLineNumber: [null, Validators.maxLength(this.builderNumberMaxLength)],
      notes: [null, Validators.maxLength(this.builderNotesMaxLength)],
      sAPAccountStatus: [null]
    });
  }

  protected initializeTranslationStrings() {
    const translateSubscription = this.translate.get([
      'notification.changesSaved',
      'notification.updateBuilderSuccess',
      'notification.saveError',
      'notification.updateBuilderError',
      'notification.invalidFormTitle',
      'notification.invalidFormMessage',
      'builder.duplicateValidationError',
      'builder.createErrorMsg',
      'notification.planInfo',
      'notification.planCreatedSuccess',
      'notification.planCreatedError'
    ]).subscribe((translations) => {
      this.changesSavedTitle = translations['notification.changesSaved'];
      this.changesSavedMessage = translations['notification.updateBuilderSuccess'];
      this.saveErrorTitle = translations['notification.saveError'];
      this.saveErrorMessage = translations['notification.updateBuilderError'];
      this.invalidFormTitle = translations['notification.invalidFormTitle'];
      this.invalidFormMessage = translations['notification.invalidFormMessage'];
      this.builderDuplicateValidationError = translations['builder.duplicateValidationError'];
      this.builderCreateErrorMsg = translations['builder.createErrorMsg'];
      this.planInfoMessage = translations['notification.planInfo'];
      this.planCreatedSuccessMsg = translations['notification.planCreatedSuccess'];
      this.planCreatedErrorMsg = translations['notification.planCreatedError'];
    });
    this.entitySubscriptions.push(translateSubscription);
  }
}

