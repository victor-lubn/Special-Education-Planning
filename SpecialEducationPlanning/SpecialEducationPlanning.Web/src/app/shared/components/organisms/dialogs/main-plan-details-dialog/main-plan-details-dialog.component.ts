import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { Builder } from 'src/app/shared/models/builder';
import { Plan } from 'src/app/shared/models/plan';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { TradeCustomerSearchTypeEnum } from '../../../../models/app-enums';
import { MainPlanDetailsFormComponent } from '../../../molecules/forms/main-plan-details-form/main-plan-details-form.component';
import { AccountLookUpComponent } from '../../account-look-up/account-look-up.component';
import { MainPlanDetailsDialogActionsEnum } from './main-plan-details-dialog.models';

@Component({
  selector: 'tdp-main-plan-details-dialog',
  templateUrl: './main-plan-details-dialog.component.html',
  styleUrls: ['./main-plan-details-dialog.component.scss']
})
export class MainPlanDetailsDialogComponent extends BaseComponent implements OnInit {

  isAccountNumberKnown: boolean = false;

  @ViewChild(MainPlanDetailsFormComponent)
  mainPlanFormComponent: MainPlanDetailsFormComponent;

  @ViewChild(AccountLookUpComponent)
  accountLookUpComponent: AccountLookUpComponent;

  planDetails: Plan;
  tradeCustomer: Builder;
  tradeCustomerType: TradeCustomerSearchTypeEnum;  
  isTradeCustomerConfirmed: boolean = false;

  constructor(
    private _dialogRef: MatDialogRef<MainPlanDetailsDialogComponent>,
    private planDetailsService: PlanDetailsService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.initializeData();
  }

  onCancel(): void {
    this._dialogRef.close({
      responseAction: MainPlanDetailsDialogActionsEnum.CANCEL
    });
  }

  onCreateUnassignedPlan(): void {
    this._dialogRef.close({
      responseAction: MainPlanDetailsDialogActionsEnum.CREATE_UNASSIGNED_PROJECT
    });
  }

  onSearchTradeAccounts(): void {
    const mainPlanForm = this.mainPlanFormComponent.entityForm;
    if (!mainPlanForm.valid) {
      mainPlanForm.markAllAsTouched();
      return;
    }

    this._dialogRef.close({
      responseAction: MainPlanDetailsDialogActionsEnum.SEARCH_TRADE_ACCOUNTS,
      tradeCustomer: mainPlanForm.value
    });
  }

  onContinue(): void {
    this.tradeCustomer = this.accountLookUpComponent.tradeCustomer;
    this.tradeCustomerType = this.accountLookUpComponent.searchType;

    this._dialogRef.close({
      responseAction: MainPlanDetailsDialogActionsEnum.CONTINUE,
      tradeCustomer: this.tradeCustomer,
      tradeCustomerType: this.tradeCustomerType
    });
  }

  onKnownAccountNumberClick(): void {
    if (!this.isAccountNumberKnown) {
      const mainPlanForm = this.mainPlanFormComponent.entityForm;
      mainPlanForm.markAsUntouched();
    }
    this.onKnownAccountNumberToggle();
  }

  onKnownAccountNumberToggle(): void {
    this.isAccountNumberKnown = !this.isAccountNumberKnown;
  }

  onChangeConfirmed(value: boolean): void {
    this.isTradeCustomerConfirmed = value;
  }

  onAccountNumberCancel(): void {
    this.tradeCustomer = null;
  }

  private initializeData(): void {
    const planDetailsSubscription = this.planDetailsService.getPlanDetails().subscribe((planDetails: Plan) => {
      this.planDetails = planDetails;
      if (!this.planDetails?.planCode) {
        this.planDetailsService.generatePlanCode();
      }
    });
    this.entitySubscriptions.push(planDetailsSubscription);

    const tradeCustomerSubscription = this.planDetailsService.getTradeCustomer().subscribe((tradeCustomer) => {
      this.tradeCustomer = tradeCustomer;
    });
    this.entitySubscriptions.push(tradeCustomerSubscription);
  }
}
