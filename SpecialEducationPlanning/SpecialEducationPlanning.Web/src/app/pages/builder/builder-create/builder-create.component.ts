import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { MatDialog } from '@angular/material/dialog';
import { MatExpansionPanel } from '@angular/material/expansion';
import { UntypedFormGroup, Validators } from '@angular/forms';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { ApiService } from '../../../core/api/api.service';
import {
  BuilderMatchTypeEnum,
  TradeCustomerFoundDialogActionsEnum,
  TradeCustomerSearchTypeEnum
} from '../../../shared/models/app-enums';
import { Builder } from '../../../shared/models/builder';
import { Plan } from '../../../shared/models/plan';
import { EducationToolMiddlewareService } from '../../../middleware/services/Education-tool-middleware.service';
import { DocumentModel } from '../../../middleware/models/document.model';
import { ComponentCanDeactivate } from '../../../shared/guards/pending-changes.guard';
import { Catalog } from '../../../shared/models/catalog.model';
import { UserInfoService } from '../../../core/services/user-info/user-info.service';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { ValidationBuilderResponse } from '../../../shared/models/validation-builder-response';
import { PlanTypeOption } from '../../../shared/models/plan-type.model';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { PageDescriptor } from '../../../core/services/url-parser/page-descriptor.model';
import { FilterDescriptor, FilterOperator } from '../../../core/services/url-parser/filter-descriptor.model';
import { EnvelopeResponse } from '../../../core/services/url-parser/envelope-response.interface';
import { BuilderFormComponent } from '../../../shared/components/forms/builder-form/builder-form.component';
import { UnassignedPlanDialogComponent } from '../../../shared/components/dialogs/unassigned-plan-dialog/unassigned-plan-dialog.component';
import { InvalidNumber } from '../../../shared/validators/is-numeric.validator';
import { EndUserFormComponent } from '../../../shared/components/forms/end-user-form/end-user-form.component';
import { BuilderGeneralComponent } from '../builder-general/builder-general.component';
import { TradeCustomerFoundDialogResponse } from 'src/app/shared/components/organisms/dialogs/trade-customer-found-dialog/trade-customer-found-dialog.component';
import { NoMatchesFoundDialogActionEnum, NoMatchesFoundDialogResponse } from 'src/app/shared/components/organisms/dialogs/no-matches-found-dialog/no-matches-found-dialog.models';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';
import { CountryControllerBase } from 'src/app/core/services/country-controller/country-controller-base';

@Component({
  selector: 'tdp-builder-create',
  templateUrl: './builder-create.component.html',
  styleUrls: ['./builder-create.component.scss']
})
export class BuilderCreateComponent extends BuilderGeneralComponent implements OnInit, OnDestroy, ComponentCanDeactivate {

  public endUserValidation: any;
  public planCreateForm: UntypedFormGroup;
  public builderAccountNumberForm: UntypedFormGroup;
  public showPlanForm: boolean;
  public assigningPlan: boolean;
  public catalogs: Catalog[];
  public planTypes: PlanTypeOption[];
  public maxAccountNumberLength: number;
  public currentValue: string;

  private noUnassignedPlansFoundTitle: string;
  private noUnassignedPlansFoundMsg: string;
  private builderAccountNumberNotEnteredTitle: string;
  private builderAccountNumberNotEnteredMessage: string;
  private builderAccountNumberNotFoundTitle: string;
  private builderAccountNumberNotFoundMessage: string;

  protected builderCreatedAsNew: boolean;
  protected builderMatchTypeEnum = BuilderMatchTypeEnum;
  private countryService: CountryControllerBase;

  @ViewChild('catalogueTrigger', { read: MatAutocompleteTrigger }) catalogueTrigger: MatAutocompleteTrigger;
  @ViewChild('planTypeTrigger', { read: MatAutocompleteTrigger }) planTypeTrigger: MatAutocompleteTrigger;
  @ViewChild('accountNumberExpansion', { static: true }) accountNumberExpansion: MatExpansionPanel;

  @ViewChild(BuilderFormComponent, { static: true })
  public builderForm: BuilderFormComponent;

  @ViewChild(EndUserFormComponent)
  public endUserForm: EndUserFormComponent;

  constructor(
    public api: ApiService,
    public notifications: NotificationsService,
    public dialogs: DialogsService,
    public translate: TranslateService,
    public communications: CommunicationService,
    public middleware: EducationToolMiddlewareService,
    public userInfo: UserInfoService,
    public matDialog: MatDialog,
    public planDetailsService: PlanDetailsService,
    private countryControllerSvc: CountryControllerService
  ) {
    super(api, notifications, dialogs, translate, communications);
    this.countryService = this.countryControllerSvc.getService();
    this.builder = null;
    this.builderCreatedAsNew = false;
    this.showPlanForm = false;
    this.maxAccountNumberLength = 11;
    this.planCreateForm = this.formBuilder.group({
      planCode: [null],
      planName: [null],
      builderTradingName: [null],
      catalogId: [null, Validators.required],
      survey: [false],
      hasEndUser: [true],
      cadFilePlanId: [null],
      planType: [null]
    });
    this.builderAccountNumberForm = this.formBuilder.group({
      accountNumber: [null, [InvalidNumber, Validators.maxLength(this.maxAccountNumberLength)]]
    });
    this.planTypes = [];
    this.assigningPlan = false;
    this.currentValue = '';
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.communications.notifyReturnHomeEnabled(false);
    this.initializeCatalogs();
    this.initializePlanType();
    this.initializeTranslationStrings();
  }

  checkValidation(): void {
    super.checkValidation();
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

  ngOnDestroy(): void {
    this.communications.notifyReturnHomeEnabled(true);
  }


  hasChanges(): boolean {
    return this.builderForm.entityForm.dirty || this.planCreateForm.dirty ||
            (this.endUserForm && this.endUserForm.entityForm.dirty) || this.builderAccountNumberForm.dirty;
  }

  public returnOrCancelBuilder(): void {
    if (this.builderCreatedAsNew) {
      this.dialogs.confirmation('dialog.genericUnsavedChanges', 'dialog.genericCancelDialog')
        .then((confirmation) => {
          if (confirmation) {
            const subscription = this.api.builders.deleteBuilder(this.builder.id)
              .subscribe(() => {
                this.builder = null;
                this.builderForm.entityForm.markAsPristine();
                this.planCreateForm.markAsPristine();
                if (this.endUserForm) {
                  this.endUserForm.entityForm.markAsPristine();
                }
                this.goBack();
              });
            this.entitySubscriptions.push(subscription);
          }
        });
    } else {
      this.goBack();
    }
  }

  public returnOrCancelPlan(): void {
    if (this.builderCreatedAsNew) {
      this.dialogs.confirmation('dialog.genericUnsavedChanges', 'dialog.genericCancelDialog')
      .then((confirmation) => {
        if (confirmation) {
          const subscription = this.api.builders.deleteBuilder(this.builder.id)
            .subscribe(() => {
              this.reopenBuilderForm();
            });
          this.entitySubscriptions.push(subscription);
        }
      });
    } else {
      this.dialogs.confirmation('dialog.genericUnsavedChanges', 'dialog.genericCancelDialog')
        .then((confirmation) => {
          if (confirmation) {
            this.reopenBuilderForm();
          }
        });
    }
  }

  public returnOrCancelEndUser(cancel: boolean): void {
    if (cancel) {
        this.endUserForm.cancelEdit(null);
        this.planCreateForm.get('hasEndUser').setValue(false);
    }
  }

  public openPlanTypeOptions(): void {
    event.stopPropagation();
    this.planTypeTrigger.openPanel();
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

  public displayCatalog(catalogId: number): string | undefined {
    let displayResult;
    if (catalogId) {
      displayResult = this.catalogs.find((catalogItem) => {
        return catalogItem.id === catalogId;
      }).name;
    }
    return displayResult;
  }

  public openCatalogueOptions(): void {
    event.stopPropagation();
    this.catalogueTrigger.openPanel();
  }

  public submitBuilderForm(submittedBuilder: Builder): void {
    if (this.builderForm.entityForm.valid) {
      submittedBuilder.accountNumber = null;
      submittedBuilder.id = 0;
      this.validateMatchingBuilders(submittedBuilder);
    } else {
      this.markFormGroupTouched(this.builderForm.entityForm);
      this.notifications.error(this.invalidFormTitle, this.invalidFormMessage);
    }
  }

  public submitFormToCreatePlan(builder: Builder): void {
    this.assigningPlan = false;
    this.submitBuilderForm(builder);
  }

  public submitFormToAssignPlan(builder: Builder): void {
    this.assigningPlan = true;
    this.submitBuilderForm(builder);
  }

  public submitPlanForms(): void {
    const planObject: Plan = {
      ...this.planCreateForm.getRawValue(),
      builderId: this.builder.id,
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
        .subscribe(
          (response) => {
            this.planCreateForm.disable();
            this.planCreateForm.markAsPristine();
            if (this.planCreateForm.get('hasEndUser').value) {
              this.endUserForm.cancelEdit(response.endUser);
            }
            this.builderForm.entityForm.markAsPristine();
            this.notifications.success(this.planInfoMessage, this.planCreatedSuccessMsg);
            this.middleware.openDocument(
              this.createDocumentModel(response)
            );
            this.navigateTo('/plan/' + response.id);
          },
          (error) => {
            this.notifications.error(this.planCreatedErrorMsg);
          }
        );
      this.entitySubscriptions.push(planCreationSubscription);
    } else {
      this.markFormGroupTouched(this.planCreateForm);
    }
  }

  public selectUnassignedPlans(): void {
    const pageDescriptor = new PageDescriptor();
    pageDescriptor.addOrUpdateFilter(new FilterDescriptor('builderId', FilterOperator.IsEqualTo, null));
    const unassignedPlansSubscription = this.api.plans.getPlansFiltered(pageDescriptor)
      .subscribe((plansFilteredResponse: EnvelopeResponse<Plan>) => {
        if (plansFilteredResponse.data.length) {
          this.openUnassignedPlansDialog(plansFilteredResponse.data);
        } else {
          this.dialogs.information(
            this.noUnassignedPlansFoundTitle,
            this.noUnassignedPlansFoundMsg
          );
          this.undoBuilderSubmit(this.builder);
        }
      });
    this.entitySubscriptions.push(unassignedPlansSubscription);
  }

  public submitGetBuilderByAccNumAndCreatePlan(): void {
    this.assigningPlan = false;
    this.validateBuilderByAccountNumber();
  }

  public submitGetBuilderByAccNumAndAssignPlan(): void {
    this.assigningPlan = true;
    this.validateBuilderByAccountNumber();
  }

  private createDocumentModel(response: Plan): DocumentModel {
    return new DocumentModel(
      true,
      response.id,
      response.planCode,
      this.catalogs.find(catalogItem => catalogItem.id === response.catalogId).code,
      this.userInfo.getUserFullName(),
      this.planCreateForm.get('builderTradingName').value,
      null,
      null,
      null,
      null,
      null,
      response.endUser ? response.endUser.firstName + ' ' + response.endUser.surname : 'Test endUser'
    );
  }

  private validateBuilderByAccountNumber(): void {
    if (!this.builderAccountNumberForm.value.accountNumber) {
      this.builderForm.entityForm.reset();
      this.dialogs.information(
        this.builderAccountNumberNotEnteredTitle,
        this.builderAccountNumberNotEnteredMessage,
      );
    } else {
      const validationSubscription = this.api.builders.validatePossibleMatchingBuildersByAccountNumber(
        this.builderAccountNumberForm.value.accountNumber
      )
      .subscribe((validationResult: ValidationBuilderResponse) => {
        if (validationResult) {
          this.openMatchDialog(validationResult, null);
        } else {
          this.builderForm.entityForm.reset();
          this.dialogs.information(
            this.builderAccountNumberNotFoundTitle,
            this.builderAccountNumberNotFoundMessage
          );
        }
      }, (error) => {
        this.notifications.error(this.builderDuplicateValidationError);
      });
      this.entitySubscriptions.push(validationSubscription);
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

  private assignExistingEndUser(existingEndUserAiep: any, planObject: Plan, existingEndUser: any) {
    let existingOtherAiepMsg: string;
    const subscription = this.translate.get([
      'dialog.endUserExistingOtherAiep'
    ], {
        AiepName: existingEndUserAiep.name
      }).subscribe((translations) => {
        existingOtherAiepMsg = translations['dialog.endUserExistingOtherAiep'];
      });
    this.entitySubscriptions.push(subscription);
    this.dialogs.confirmation('dialog.endUserExistingTitle', existingOtherAiepMsg).then((confirmation) => {
      if (confirmation) {
        planObject.endUserId = existingEndUser.id;
        this.submitPlan(planObject);
      }
    });
  }

  private openUnassignedPlansDialog(planList: Plan[]) {
    const unassignedPlansDialogRef = this.matDialog.open(UnassignedPlanDialogComponent, {
      data: {
        planListInput: planList
      }
    });
    const unassignedPlanDialogSubscription = unassignedPlansDialogRef.afterClosed()
      .subscribe((selectedPlan: Plan) => {
        if (selectedPlan) {
          const assignSubscription = this.api.plans.assignBuilderToPlan(selectedPlan.id, this.builder.id)
            .subscribe((response: Plan) => {
              // this.notifications.success(this.assignBuilderSuccessMessage);
              this.builderForm.entityForm.markAsPristine();
              this.builderAccountNumberForm.markAsPristine();
              this.navigateTo('/builder/' + response.builderId);
            }, (error) => {
              // this.notifications.error(this.assignBuilderErrorMessage);
              this.undoBuilderSubmit(this.builder);
            });
          this.entitySubscriptions.push(assignSubscription);
        } else {
          this.undoBuilderSubmit(this.builder);
        }
      });
    this.entitySubscriptions.push(unassignedPlanDialogSubscription);
  }

  private validateMatchingBuilders(builder: Builder): void {
    const validationSubscription  = this.api.builders.validatePossibleMatchingBuilders(builder)
      .subscribe((validationResult: ValidationBuilderResponse) => {
        if (validationResult.builders.length) {
          this.openMatchDialog(validationResult, builder);
        } else {
          this.openNoMatchDialog(builder);
        }
      }, (error) => {
        this.notifications.error(this.builderDuplicateValidationError);
      });
    this.entitySubscriptions.push(validationSubscription);
  }

  private openMatchDialog(validationResponse: ValidationBuilderResponse, inputTradeCustomer: Builder): void {
    this.dialogs
      .existingTradeCustomer(validationResponse, this.countryService)
      .then((dialogResponse: TradeCustomerFoundDialogResponse) => {
        switch(dialogResponse.responseAction) {
          case TradeCustomerFoundDialogActionsEnum.USE_ACCOUNT:
            this.planDetailsService.setTradeCustomer(dialogResponse.selectedTradeCustomer);
            this.dialogs.openCreatePlanModal();
            break;
          case TradeCustomerFoundDialogActionsEnum.CREATE_NEW:
            this.openNoMatchDialog(inputTradeCustomer);
            break;
          case TradeCustomerFoundDialogActionsEnum.CANCEL:
            this.planDetailsService.resetPlanDetails();
            break;
          case TradeCustomerFoundDialogActionsEnum.BACK:
            // TODO Open Main PlanDetails pop-up;
            break;
        }
      });
  }

  private openNoMatchDialog(inputTradeCustomer: Builder): void {
    this.planDetailsService.setTradeCustomer(inputTradeCustomer);
    this.dialogs
      .createLocalCashAccount()
      .then((dialogResponse: NoMatchesFoundDialogResponse) => {
        switch(dialogResponse.responseAction) {
          case NoMatchesFoundDialogActionEnum.BACK:
            // TODO Open Main Plan Details pop-up;
            break;
          case NoMatchesFoundDialogActionEnum.CREATE_LOCAL_CASH_ACCOUNT:
            this.createBuilderInDatabase(dialogResponse.tradeCustomerFormValue);
            break;
        }
      });
  }

  private createOrAssignBuilder(builder: Builder, searchType?: TradeCustomerSearchTypeEnum): void {
    if (searchType && searchType === TradeCustomerSearchTypeEnum.SapCredit) {
      this.createBuilderInDatabase(builder);
    } else {
      this.assignBuilderToCurrentAiep(builder);
    }
  }

  private createBuilderInDatabase(builder: Builder) {
    const createBuilderSubscription = this.api.builders.createBuilder(builder)
      .subscribe((response) => {
        this.planDetailsService.setTradeCustomer(response);
        this.dialogs.openCreatePlanModal();
      }, (error) => {
        this.notifications.error(this.builderCreateErrorMsg);
      });
    this.entitySubscriptions.push(createBuilderSubscription);
  }

  private assignBuilderToCurrentAiep(builder: Builder) {
    const assignBuilderSuscription =
      this.api.builders.assignBuilderToCurrentUserAiep(builder.id)
        .subscribe(() => {
          // this.finishBuilderForm(builder);
        }, (error) => {
          // this.notifications.error(this.assignBuilderErrorMessage);
        });
    this.entitySubscriptions.push(assignBuilderSuscription);
  }

  private undoBuilderSubmit(builder?: Builder): void {
    if (this.builderCreatedAsNew) {
      const subscription = this.api.builders.deleteBuilder(builder.id)
        .subscribe(() => {
          this.reopenBuilderForm();
        });
      this.entitySubscriptions.push(subscription);
    } else {
      this.reopenBuilderForm();
    }
  }

  private reopenBuilderForm(): void {
    if (this.planCreateForm) {
      this.planCreateForm.reset();
    }
    this.builderForm.entityForm.enable();
    this.builderForm.entityForm.controls['id'].setValue(0);
    this.builderAccountNumberForm.reset();
    this.builderAccountNumberForm.enable();
    this.builderForm.entityForm.disable();
    this.editing = true;
    this.showPlanForm = false;
    this.builderCreatedAsNew = false;
  }

  private finishBuilderForm(builder: Builder) {
    this.builder = builder;
    this.builderForm.patchForm(builder);
    if (this.assigningPlan) {
      this.selectUnassignedPlans();
    } else {
      this.initPlanForm();
    }
  }

  private initPlanForm(): void {
    const planCodeSubscription = this.api.plans.generatePlanCode()
      .subscribe((planCode: string) => {
        this.planCreateForm.get('planCode').patchValue(planCode);
        this.planCreateForm.get('planCode').disable();
        this.planCreateForm.get('builderTradingName')
          .patchValue(this.builder.tradingName);
        this.planCreateForm.get('builderTradingName').disable();
        this.accountNumberExpansion.close();
        this.builderAccountNumberForm.reset();
        this.builderAccountNumberForm.disable();
        this.planCreateForm.get('survey').patchValue(false);
        this.planCreateForm.get('hasEndUser').patchValue(true);
        this.showPlanForm = true;
      });
    this.entitySubscriptions.push(planCodeSubscription);
  }

  private initializeCatalogs() {
    const catalogsSubscription = this.api.plans.getCatalogs()
      .subscribe((result) => {
        this.catalogs = result;
      });
    this.entitySubscriptions.push(catalogsSubscription);
  }

  private initializePlanType() {
    const planTypeSubscription = this.api.plans.getPlanTypes()
      .subscribe((result) => {
        this.planTypes = result;
      });
    this.entitySubscriptions.push(planTypeSubscription);
  }

  private validateEndUserMandatoryFieldsFilled(): boolean {
    if (this.endUserForm) {
      if (this.endUserForm.entityForm.get('surname').value
        && this.endUserForm.entityForm.get('postcode').value
        && this.endUserForm.entityForm.get('address1').value) {
          return true;
      }
    }
    return false;
  }

  protected initializeTranslationStrings(): void {
    const translationsSubscription = this.translate.get([
      'dialog.noUnassignedPlansFoundTitle',
      'dialog.noUnassignedPlansFoundMsg',
      'dialog.builderAccountNumberNotEntered.title',
      'dialog.builderAccountNumberNotEntered.message',
      'dialog.builderAccountNumberNotFound.title',
      'dialog.builderAccountNumberNotFound.message'
    ]).subscribe(translations => {
      this.noUnassignedPlansFoundTitle = translations['dialog.noUnassignedPlansFoundTitle'];
      this.noUnassignedPlansFoundMsg = translations['dialog.noUnassignedPlansFoundMsg'];
      this.builderAccountNumberNotEnteredTitle = translations['dialog.builderAccountNumberNotEntered.title'];
      this.builderAccountNumberNotEnteredMessage = translations['dialog.builderAccountNumberNotEntered.message'];
      this.builderAccountNumberNotFoundTitle = translations['dialog.builderAccountNumberNotFound.title'];
      this.builderAccountNumberNotFoundMessage = translations['dialog.builderAccountNumberNotFound.message'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }
}


