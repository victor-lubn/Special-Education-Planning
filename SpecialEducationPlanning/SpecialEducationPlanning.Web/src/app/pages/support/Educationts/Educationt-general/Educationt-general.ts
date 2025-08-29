import { Component, OnInit, ViewChild } from '@angular/core';

import { NotificationsService } from 'angular2-notifications';
import { ApiService } from '../../../../core/api/api.service';
import { ComponentCanDeactivate } from '../../../../shared/guards/pending-changes.guard';
import { AiepFormComponent } from '../../../../shared/components/forms/Aiep-form/Aiep-form.component';
import { UntypedFormGroup } from '@angular/forms';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-Aiep-details',
  template: `<div></div>`
})
export class AiepGeneralComponent extends FormComponent implements ComponentCanDeactivate, OnInit{

  public AiepValidation: UntypedFormGroup;

  @ViewChild(AiepFormComponent)
  protected AiepForm: AiepFormComponent;

  protected readonly ipAddressRegex = '^(([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\.){3}([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])$';

  //Translations strings
  protected AiepInfoMessage;
  protected AiepCreatedSuccessMessage;
  protected AiepCreatedErrorMessage;
  protected AiepUpdatedSuccessMessage;
  protected AiepUpdatedErrorMessage;

  constructor(
    protected api: ApiService,
    protected notifications: NotificationsService,
    protected translate: TranslateService
  ) {
    super();
    this.checkAiepValidation();
    this.AiepInfoMessage = '';
    this.AiepCreatedSuccessMessage = '';
    this.AiepCreatedErrorMessage = '';
    this.AiepUpdatedSuccessMessage = '';
    this.AiepUpdatedErrorMessage = '';
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
  }

  hasChanges() {
    return this.AiepForm.entityForm.dirty;
  }

  public checkAiepValidation(): void {
    this.AiepValidation = this.formBuilder.group({
      AiepCode: [null, Validators.required],
      name: [null, Validators.required],
      email: [null, [Validators.required, Validators.email]],
      country: [null],
      postcode: ['', [Validators.required]],
      address1: [null, Validators.required],
      address2: [null],
      address3: [null],
      phoneNumber: [null],
      faxNumber: [null],
      areaName: [null, Validators.required],
      ipAddress: [null, [Validators.pattern(this.ipAddressRegex)]],
      managerName: [null],
      downloadLimit: [0, [Validators.min(0), Validators.max(100), Validators.required]],
      downloadSpeed: [0, [Validators.min(0), Validators.required]],
      htmlEmail: [false]
    });
  }

  protected initializeTranslationStrings() {
    const subscription = this.translate.get([
      'notification.AiepInfo',
      'notification.AiepCreatedSuccess',
      'notification.AiepCreatedError',
      'notification.AiepUpdatedSuccess',
      'notification.AiepUpdatedError'
    ]).subscribe((translations) => {
      this.AiepInfoMessage = translations['notification.AiepInfo'];
      this.AiepCreatedSuccessMessage = translations['notification.AiepCreatedSuccess'];
      this.AiepCreatedErrorMessage = translations['notification.AiepCreatedError'];
      this.AiepUpdatedSuccessMessage = translations['notification.AiepUpdatedSuccess'];
      this.AiepUpdatedErrorMessage = translations['notification.AiepUpdatedError'];
    });
    this.entitySubscriptions.push(subscription);
  }
}

