import { Component, Input, OnInit } from '@angular/core';
import { CountryControllerBase } from 'src/app/core/services/country-controller/country-controller-base';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';

@Component({
  selector: 'tdp-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss']
})
export class AccountDetailsComponent implements OnInit {

  @Input()
  tradeCustomer;
  countrySvc: CountryControllerBase;
  constructor(private countryControllerSvc: CountryControllerService) {
    this.countrySvc = this.countryControllerSvc.getService()
  }

  ngOnInit(): void {
  }

}
