import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { Builder } from 'src/app/shared/models/builder';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { TradeCustomerFormComponent } from '../../../molecules/forms/trade-customer-form/trade-customer-form.component';
import { NoMatchesFoundDialogActionEnum } from './no-matches-found-dialog.models';


@Component({
  selector: 'tdp-no-matches-found-dialog',
  templateUrl: './no-matches-found-dialog.component.html',
  styleUrls: ['./no-matches-found-dialog.component.scss']
})
export class NoMatchesFoundDialogComponent extends BaseComponent implements OnInit {

  tradeCustomer: Builder;

  @ViewChild(TradeCustomerFormComponent, { static: true })
  tradeCustomerFormComponent: TradeCustomerFormComponent;

  constructor(
    private _dialogRef: MatDialogRef<NoMatchesFoundDialogComponent>,
    private planDetailsService: PlanDetailsService
  ) {
    super();
   }

  ngOnInit(): void {
    this.initializeData();
  }

  onClose(): void {
    this._dialogRef.close({
      responseAction: NoMatchesFoundDialogActionEnum.CLOSE
    });
  }

  onBack(): void {
    const tradeCustomerForm = this.tradeCustomerFormComponent.entityForm;
    this.planDetailsService.setTradeCustomer(tradeCustomerForm.value);
    this._dialogRef.close({
      responseAction: NoMatchesFoundDialogActionEnum.BACK
    });
  }

  onCreateLocalCashAccount(): void {
    const tradeCustomerForm = this.tradeCustomerFormComponent.entityForm;
    if (tradeCustomerForm.invalid) {
      tradeCustomerForm.markAllAsTouched();
      return;
    } else {
      this._dialogRef.close({
        responseAction: NoMatchesFoundDialogActionEnum.CREATE_LOCAL_CASH_ACCOUNT,
        tradeCustomerFormValue: tradeCustomerForm.value
      });
    }
  }

  private initializeData(): void {
    const tradeCustomerSubscription = this.planDetailsService.getTradeCustomer().subscribe((tradeCustomer: Builder) => {
      this.tradeCustomer = tradeCustomer;
    });
    this.entitySubscriptions.push(tradeCustomerSubscription);
  }
}
