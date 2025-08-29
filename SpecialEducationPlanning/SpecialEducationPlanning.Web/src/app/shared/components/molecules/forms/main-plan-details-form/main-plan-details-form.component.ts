import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { Builder } from '../../../../models/builder';
import { getPostcodeValidator } from '../../../../validators/control-validators/postcode';

@Component({
  selector: 'tdp-main-plan-details-form',
  templateUrl: './main-plan-details-form.component.html',
  styleUrls: ['./main-plan-details-form.component.scss']
})
export class MainPlanDetailsFormComponent extends FormComponent implements OnInit, OnChanges {

  @Input()
  tradeCustomer: Builder;

  constructor() {
    super();
  }

  ngOnInit(): void {
    this.initializeForm(this.tradeCustomer);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.tradeCustomer) {
      this.initializeForm(changes.tradeCustomer.currentValue);
    }
  }

  private initializeForm(tradeCustomer: Builder): void {
    this.entityForm = this.formBuilder.group({
      tradingName: [tradeCustomer?.tradingName, Validators.required],
      postcode: [tradeCustomer?.postcode, [Validators.required, this.getPostcodeValidation()]],
      address1: [tradeCustomer?.address1, Validators.required],
      address2: [tradeCustomer?.address2],
      address3: [tradeCustomer?.address3]
    });
  }

  private getPostcodeValidation() {
    return getPostcodeValidator().getValidator;
  }

}
