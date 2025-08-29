import { ChangeDetectorRef, Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { Builder } from 'src/app/shared/models/builder';
import { getPostcodeValidator } from '../../../../validators/control-validators/postcode';

@Component({
  selector: 'tdp-trade-customer-form',
  templateUrl: './trade-customer-form.component.html',
  styleUrls: ['./trade-customer-form.component.scss']
})
export class TradeCustomerFormComponent extends FormComponent implements OnInit, OnChanges {

  @Input()
  tradeCustomer: Builder;

  protected readonly notesMaxLength: number = 500;

  constructor(
    private cdr: ChangeDetectorRef
  ) {
    super();
   }

  ngOnInit(): void {
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.tradeCustomer) {
      this.initializeForm();
      this.cdr.detectChanges();
    }
  }

  private initializeForm(): void {
    if (!this.tradeCustomer) {
      this.entityForm = this.formBuilder.group({
        tradingName: [null, Validators.required],
        name: [null],
        postcode: [null, [Validators.required, this.getPostcodeValidation()]],
        address1: [null, Validators.required],
        address2: [null],
        address3: [null],
        mobileNumber: [null],
        landlineNumber: [null],
        email: [null, Validators.email],
        notes: [null, Validators.maxLength(this.notesMaxLength)]
      })
    } else {
      this.entityForm = this.formBuilder.group({
        tradingName: [this.tradeCustomer.tradingName, Validators.required],
        name: [this.tradeCustomer.name],
        postcode: [this.tradeCustomer.postcode, [Validators.required, this.getPostcodeValidation()]],
        address1: [this.tradeCustomer.address1, Validators.required],
        address2: [this.tradeCustomer.address2],
        address3: [this.tradeCustomer.address3],
        mobileNumber: [this.tradeCustomer.mobileNumber],
        landlineNumber: [this.tradeCustomer.landLineNumber],
        email: [this.tradeCustomer.email, Validators.email],
        notes: [this.tradeCustomer.notes, Validators.maxLength(this.notesMaxLength)]
      })
    }
  }

  private getPostcodeValidation() {
    return getPostcodeValidator().getValidator;
  }

}
