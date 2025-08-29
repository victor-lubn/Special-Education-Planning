import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { EndUserFormComponent } from './../../../shared/components/forms/end-user-form/end-user-form.component';
import { EndUser } from './../../../shared/models/end-user';
import { FormComponent } from '../../../shared/base-classes/form-component';
import { Builder } from '../../../shared/models/builder';
import { ApiService } from '../../../core/api/api.service';
import { Plan } from '../../../shared/models/plan';
import { ComponentCanDeactivate } from '../../../shared/guards/pending-changes.guard';
import { Catalog } from '../../../shared/models/catalog.model';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { PlanTypeOption } from '../../../shared/models/plan-type.model';
import { UserInfoService } from '../../../core/services/user-info/user-info.service';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { EducationToolService } from '../../../core/Education-tool/Education-tool.service';
import { EducationToolType } from '../../../shared/models/app-enums';
import { ThreeDCDocumentModel } from '../../../middleware/models/ThreeDCDocumentModel';
import { FusionDocumentModel } from '../../../middleware/models/FusionDocumentModel';
import { EducationToolMiddlewareService } from '../../../middleware/services/Education-tool-middleware.service';

@Component({
  selector: 'tdp-plan-create',
  templateUrl: './plan-create.component.html',
  styleUrls: ['./plan-create.component.scss'],
})
export class PlanCreateComponent extends FormComponent implements OnInit, OnDestroy, ComponentCanDeactivate  {

  public endUserValidation: any;
  public planCreateForm: UntypedFormGroup;
  public catalogs: Catalog[];
  public planTypes: PlanTypeOption[];

  public builderId: number;
  public builder: Builder;
  public endUser: EndUser;
  public surnameMaxLength: number;
  public currentValue: string;


  //Translation strings
  protected planInfoMessage = '';
  protected planCreatedSuccessMsg = '';
  protected planCreatingError = '';

  @ViewChild('catalogueTrigger', { read: MatAutocompleteTrigger, static: true }) catalogueTrigger: MatAutocompleteTrigger;
  @ViewChild('planTypeTrigger', { read: MatAutocompleteTrigger, static: true }) planTypeTrigger: MatAutocompleteTrigger;

  @ViewChild(EndUserFormComponent)
  endUserForm: EndUserFormComponent;

  constructor(
    protected route: ActivatedRoute,
    protected notifications: NotificationsService,
    protected api: ApiService,
    protected middleware: EducationToolMiddlewareService,
    protected dialog: DialogsService,
    protected translate: TranslateService,
    protected userInfo: UserInfoService,
    protected communication: CommunicationService,
    protected EducationToolService: EducationToolService
  ) {
    super();
    this.currentValue = '';
    this.surnameMaxLength = 20;
    this.planCreateForm = this.formBuilder.group({
      planCode: [null],
      planName: [null],
      cadFilePlanId: [null],
      builderTradingName: [null],
      catalogId: [null, Validators.required],
      planType: [null],
      survey: [false],
      hasEndUser: [true]
    });
    this.planTypes = [];
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
    this.communication.notifyClearHomeFilters(false);
    this.communication.notifyAiepSelectorEnabled(false);

    const catalogsSubscription = this.api.plans.getCatalogs()
      .subscribe((result) => {
        this.catalogs = result;
      });
    this.entitySubscriptions.push(catalogsSubscription);
    const planTypeSubscription = this.api.plans.getPlanTypes()
      .subscribe((result) => {
        this.planTypes = result;
      });
    this.entitySubscriptions.push(planTypeSubscription);
    this.planCreateForm.get('planCode').disable();
    this.planCreateForm.get('builderTradingName').disable();
    const routeSubscription = this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.builderId = +queryParams.get('builderId');
      if (this.builderId) {
        const builderSubscription = this.api.builders.getBuilder(this.builderId)
          .subscribe((builder) => {
            this.builder = builder;
            this.planCreateForm.get('builderTradingName').patchValue(builder['tradingName']);
          });
        this.entitySubscriptions.push(builderSubscription);
      }
    });
    this.entitySubscriptions.push(routeSubscription);
    const planCodeSubscription = this.api.plans.generatePlanCode()
      .subscribe((planCode: string) => {
        this.planCreateForm.get('planCode').patchValue(planCode);
      });
    this.entitySubscriptions.push(planCodeSubscription);
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
          country: [null],
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
    return this.planCreateForm.dirty || (this.endUserForm && this.endUserForm.entityForm.dirty);
  }

  public openCatalogueOptions(): void {
    event.stopPropagation();
    this.catalogueTrigger.openPanel();
  }

  public openPlanTypeOptions(): void {
    event.stopPropagation();
    this.planTypeTrigger.openPanel();
  }

  // FIXME cancelEdit(null)
  public cancelEndUser(): void {
    if (this.endUserForm.entityForm.dirty) {
      this.dialog.confirmation('dialog.genericUnsavedChanges', 'dialog.genericCancelDialog')
        .then((confirmation) => {
          if (confirmation) {
            this.endUserForm.cancelEdit(null);
            this.planCreateForm.get('hasEndUser').setValue(false);
          }
        });
    } else {
      this.endUserForm.cancelEdit(null);
      this.planCreateForm.get('hasEndUser').setValue(false);
    }
  }

  public displayCatalog(catalogId: number): string | undefined {
    let displayResult;
    if (catalogId) {
      displayResult = this.catalogs.find((catalogItem) => {
        return catalogItem.id === catalogId;
      }).name;
    }
    return displayResult;
  }

  public displayPlanType(planTypeString): string | undefined {
    let displayResult;
    if (planTypeString) {
      displayResult = this.planTypes.find((planTypeValue: PlanTypeOption) => planTypeValue.value == planTypeString);
      this.translate.get(`${planTypeString}`).subscribe((translations) => {
        this.currentValue = translations;
      })}
    return this.currentValue;
  }

  // Workflow starts here!!
  public submitForms(): void {
    const planObject: Plan = {
      ...this.planCreateForm.value,
      builderId: this.builderId ? this.builderId : null,
      planCode: this.planCreateForm.get('planCode').value,
      planType: this.currentValue,
      projectId: 0,
      id: 0
    };
    if (this.planCreateForm.get('hasEndUser').value) {
      this.validateExistingEndUser(planObject);
    } else {
      this.submitPlan(planObject);
    }
  }

  public submitPlan(planObject: Plan): void {
    if (this.planCreateForm.valid) {
      const planCreationSubscription = this.api.plans.createSinglePlan(planObject)
        .subscribe((response) => {
          this.planCreateForm.disable();
          this.planCreateForm.markAsPristine();
          if (this.planCreateForm.get('hasEndUser').value) {
            this.endUserForm.cancelEdit(response.endUser);
          }
          this.notifications.success(this.planInfoMessage, this.planCreatedSuccessMsg);


          if (planObject.EducationOrigin === EducationToolType.THREE_DC) {
            const documentModel = this.EducationToolService.generateDocumentModel<ThreeDCDocumentModel>(
              planObject.id,
              planObject.planName,
              planObject.planCode,
              planObject.builderTradingName,
              this.catalogs.find(catalogItem => catalogItem.id === planObject.catalogId).code,
              response.EducationOrigin,
              planObject.catalogId,
              planObject.versions[0],
              response.endUser);
            this.EducationToolService.open3dcService.getAuthTokenAndOpen3DCInNewWindow(documentModel);
          } else {
            const documentModel = this.EducationToolService.generateDocumentModel<FusionDocumentModel>(
              planObject.id,
              planObject.planName,
              planObject.planCode,
              planObject.builderTradingName,
              this.catalogs.find(catalogItem => catalogItem.id === planObject.catalogId).code,
              response.EducationOrigin,
              planObject.catalogId,
              planObject.versions[0],
              response.endUser);
            this.middleware.openDocument(documentModel);
          }

          this.navigateTo('/plan/' + response.id);
        }, (error) => {
          this.notifications.error(this.planCreatingError);
        });
      this.entitySubscriptions.push(planCreationSubscription);
    } else {
      this.markFormGroupTouched(this.planCreateForm);
    }
  }

  private validateExistingEndUser(planObject: Plan) {
    const submittedEndUser = this.endUserForm.entityForm.value;
    const endUserValidationSubscription = this.api.plans.validateExistingEndUsers(submittedEndUser)
      .subscribe((response) => {
        const existingEndUser = response.endUser;
        const existingEndUserAiep = response.Aiep;
        if (!existingEndUser) {
          planObject.endUser = {
            ...submittedEndUser
          };
          this.submitPlan(planObject);
        } else {
          this.assignExistingEndUser(existingEndUserAiep, planObject, existingEndUser);
        }
      });
    this.entitySubscriptions.push(endUserValidationSubscription);
  }

  private assignExistingEndUser(Aiep: any, planObject: Plan, endUser: any) {
    let existingOtherAiepMsg: string;
    const subscription = this.translate.get([
      'dialog.endUserExistingOtherAiep'
    ], {
        AiepName: Aiep.name
      }).subscribe((translations) => {
        existingOtherAiepMsg = translations['dialog.endUserExistingOtherAiep'];
        this.dialog.confirmation('dialog.endUserExistingTitle', existingOtherAiepMsg, '400px').then((confirmation) => {
          if (confirmation) {
            planObject.endUserId = endUser.id;
            this.submitPlan(planObject);
          }
        });
      });
    this.entitySubscriptions.push(subscription);
  }

  protected initializeTranslationStrings(){
    const translateSubscription = this.translate.get([
      'notification.planInfo',
      'notification.planCreatedSuccess',
      'notification.planCreatingError'
    ]).subscribe((translations) => {
        this.planInfoMessage = translations['notification.planInfo'];
        this.planCreatedSuccessMsg = translations['notification.planCreatedSuccess'];
        this.planCreatingError = translations['notification.planCreatingError'];
      });
  this.entitySubscriptions.push(translateSubscription);
  }
}


