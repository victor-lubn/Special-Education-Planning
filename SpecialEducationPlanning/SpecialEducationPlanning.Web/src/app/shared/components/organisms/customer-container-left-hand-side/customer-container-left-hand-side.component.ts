import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { Builder } from '../../../models/builder';
import { CustomerFormComponent } from '../../molecules/customer-form/customer-form.component';
@Component({
  selector: 'tdp-customer-container-left-hand-side',
  templateUrl: './customer-container-left-hand-side.component.html',
  styleUrls: ['./customer-container-left-hand-side.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CustomerContainerLeftHandSideComponent implements OnInit {
  // Address info
  @Input() customerData: Builder

  @Output() submitData = new EventEmitter<any>()

  @ViewChild(CustomerFormComponent) customerForm: CustomerFormComponent;

  /*----------  Functional properties  ----------*/
  customerInfoUpdateState: boolean = false
  customerInfoForm: UntypedFormGroup

  ngOnInit(): void {

  }

  updateCustomerInfo() {
    this.customerInfoUpdateState = true
  }

  onSubmit(values: any) {
    this.customerData = {
      ...this.customerData,
      ...values
    }
    this.customerInfoUpdateState = false
    this.submitData.emit(this.customerData)
  }


}
