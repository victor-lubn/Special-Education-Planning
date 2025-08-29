import { Component, Input, OnInit } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { SelectOptionsService } from 'src/app/core/services/select-options/select-options.service';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { Address } from 'src/app/shared/models/address';
import { environment } from 'src/environments/environment';
import { SelectOptionInterface } from '../../atoms/select/select.component';

@Component({
  selector: 'tdp-partial-address-form',
  templateUrl: './partial-address-form.component.html',
  styleUrls: ['./partial-address-form.component.scss']
})
export class PartialAddressFormComponent extends FormComponent implements OnInit {
  @Input('formGroup') entityForm: UntypedFormGroup; // eslint-disable-line @angular-eslint/no-input-rename
  @Input() singleLine: boolean = false;
  @Input() width: string = '100%';
  @Input() gap: string = '1.5rem';
  countryCode: string = environment.defaultCountrySelector;
  postcode$: Observable<string>;
  postcodeCountries: SelectOptionInterface[] | undefined;
  constructor(private selectOptionsSvc: SelectOptionsService) {
    super();
  }

  ngOnInit(): void {
    this.postcode$ = this.entityForm.get('postcode').valueChanges;
    const postcodeCountriesSubscription = this.selectOptionsSvc.getPostCodeCountries$()
      .subscribe(postcodeCountries => {
        this.postcodeCountries = postcodeCountries;
      });
    this.entitySubscriptions.push(postcodeCountriesSubscription);
  }

  onSelectedAddress(event: Address): void {
    this.entityForm.patchValue({
      address1: event.addressLine1,
      address2: event.addressLine2,
      address3: event.addressLine3,
      postcode: event.postalCode
    });
  }

  countryCodeHandler(countryCode: string): void {
    this.countryCode = countryCode;
    this.entityForm.patchValue({
      country: countryCode
    });
  }
}
