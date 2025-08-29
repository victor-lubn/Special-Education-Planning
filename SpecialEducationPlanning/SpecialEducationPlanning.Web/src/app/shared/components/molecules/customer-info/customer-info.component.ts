import { Component, Input, OnInit } from '@angular/core';
import { CountryControllerBase } from 'src/app/core/services/country-controller/country-controller-base';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';
import { iconNames } from '../../atoms/icon/icon.component';

export interface CustomerDataInterface {
  tradingName: string;
  name: string;
  address1: string;
  address2: string;
  address3: string;
  postcode: string;
  landLineNumber: string;
  mobileNumber: string;
  email: string;
}

@Component({
  selector: 'tdp-customer-info',
  templateUrl: './customer-info.component.html',
  styleUrls: ['./customer-info.component.scss']
})
export class CustomerInfoComponent {
  @Input() customerData: CustomerDataInterface;
  countrySvc: CountryControllerBase;
  constructor(private countryControllerSvc: CountryControllerService) {
    this.countrySvc = this.countryControllerSvc.getService();
  }
  // Icons
  telephoneIcon: string = iconNames.size24px.TELEPHONE_BLACK;
  mobileIcon: string = iconNames.size24px.MOBILE_BLACK;
  emailIcon: string = iconNames.size24px.EMAIL_BLACK;
}
