import { Component, DoCheck, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { getPostcodeValidator } from '../../../validators/control-validators/postcode';
import { CustomerDataInterface } from '../customer-info/customer-info.component';



@Component({
  selector: 'tdp-customer-form',
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.scss']
})
export class CustomerFormComponent implements DoCheck, OnInit {
  @Input() initialValues: CustomerDataInterface
  @Output() submitForm = new EventEmitter<any>();

  // Error messages
  tradingNameErrorMessage: string
  addressOneErrorMessage: string
  postcodeErrorMessage: string
  landlineErrorMessage: string
  mobileErrorMessage: string
  emailErrorMessage: string

  /*----------  Functional properties  ----------*/
  customerInfoUpdateState: boolean = false
  customerInfoForm: UntypedFormGroup

  constructor(private fb: UntypedFormBuilder, public translate: TranslateService) {

  }

  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
    this.customerInfoForm = this.fb.group({
      tradingName: [this.initialValues.tradingName, Validators.required],
      name: [this.initialValues.name],
      address1: [this.initialValues.address1, Validators.required],
      address2: [this.initialValues.address2],
      address3: [this.initialValues.address3],
      postcode: [this.initialValues.postcode, [Validators.required, this.getPostcodeValidation()]],
      landLineNumber: [this.initialValues.landLineNumber, Validators.pattern('[- +()0-9]+')],
      mobileNumber: [this.initialValues.mobileNumber, Validators.pattern('[- +()0-9]+')],
      email: [this.initialValues.email, Validators.email],
    })
  }

  onSubmit() {
    this.customerInfoUpdateState = false

    if (this.customerInfoForm.valid) {
      this.submitForm.emit(this.customerInfoForm.value)
    }
  }

  private checkRequiredField(field: string, errorMessage: string) {
    if (this.customerInfoForm.controls[field].touched && this.customerInfoForm.controls[field].invalid) {
      this[errorMessage] = this.translate.instant('validationErrors.required')
    } else {
      this[errorMessage] = undefined
    }
  }

  private checkEmailField(field: string, errorMessage: string) {
    if (this.customerInfoForm.controls[field].touched && this.customerInfoForm.controls[field].invalid) {
      this[errorMessage] = this.translate.instant('validationErrors.mustBeEmail')
    } else {
      this[errorMessage] = undefined
    }
  }
  private checkPhonesField(field: string, errorMessage: string) {
    if (this.customerInfoForm.controls[field].touched && this.customerInfoForm.controls[field].invalid) {
      this[errorMessage] = this.translate.instant('validationErrors.invalidPhoneNumber')
    } else {
      this[errorMessage] = undefined
    }
  }

  private getPostcodeValidation() {
    const postcodeValidator = getPostcodeValidator().getValidator;
    return postcodeValidator;
  }

  ngDoCheck(): void {
    this.checkRequiredField('postcode', 'postcodeErrorMessage')
    this.checkRequiredField('tradingName', 'tradingNameErrorMessage')
    this.checkEmailField('email', 'emailErrorMessage')
    this.checkPhonesField('landLineNumber', 'landlineErrorMessage')
    this.checkPhonesField('mobileNumber', 'mobileErrorMessage')
  }
}


