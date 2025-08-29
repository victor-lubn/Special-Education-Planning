import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Validators } from '@angular/forms';

import { FormComponent } from '../../../base-classes/form-component';
import { EndUser } from '../../../models/end-user';
import { Address } from '../../../models/address';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { SelectableOption } from '../../selector/selector.component';
import { ApiService } from '../../../../core/api/api.service';
import { Title } from '../../../models/title.model';
import { COUNTRY_SELECTOR_OPTIONS } from '../../../models/country-selector-options';
import { environment } from './../../../../../environments/environment';

@Component({
  selector: 'tdp-end-user-form',
  templateUrl: 'end-user-form.component.html',
  styleUrls: ['end-user-form.component.scss']
})
export class EndUserFormComponent extends FormComponent implements OnInit {
  public endUserTitles: SelectableOption<number, string>[];

  public countriesSelectionOptions: any;
  public selectedCountry: string;
  public displayDefault?: any;

  @Input()
  public endUserValidation: any;

  @Input()
  public endUser: EndUser;

  @Input()
  public showCountrySelector: boolean;

  @Output()
  public formHasChanges = new EventEmitter<boolean>();

  @Output()
  public endUserEditClicked = new EventEmitter<void>();

  @Output()
  public endUserSubmitted = new EventEmitter<EndUser>();

  @Output()
  public endUserCanceled = new EventEmitter<boolean>();

  constructor(private dialogs: DialogsService, private api: ApiService) {
    super();
    this.endUserTitles = [];
    this.countriesSelectionOptions = COUNTRY_SELECTOR_OPTIONS;
  }

  ngOnInit(): void {
    if(this.endUserValidation) {
      this.entityForm = this.formBuilder.group({
        ...this.endUserValidation
      });
    }
    else {
      this.entityForm = this.formBuilder.group({
        titleId: [{value: null, disabled: true }],
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
    });
    }
    this.initializeEndUserFormControl();
    this.initializeTitleSelector();
    this.getCountrySelectorDefault();
  }

  public onSelectedAddress(event: Address): void {
    this.entityForm.patchValue({
      address1: event.addressLine1,
      address2: event.addressLine2,
      address3: event.addressLine3,
      postcode: event.postalCode
    });
  }

  public submitForm(): void {
    this.endUserSubmitted.emit(this.entityForm.value);
  }

  public enableButtonClick(): void {
    this.endUserEditClicked.emit();
  }

  public cancelEndUser(): void {
    if (this.entityForm.dirty) {
      this.dialogs
        .confirmation(
          'dialog.genericUnsavedChanges',
          'dialog.genericCancelDialog'
        )
        .then(confirmation => {
          this.endUserCanceled.emit(confirmation);
        });
    } else {
      this.endUserCanceled.emit(false);
    }
  }

  private patchForm() {
    this.entityForm.patchValue(this.endUser);
    this.entityForm.disable();
  }

  public initializeEndUserFormControl() {
    const valueChangesSubscription = this.entityForm.valueChanges.subscribe(
      () => {
        this.formHasChanges.emit(this.entityForm.dirty);
      }
    );
    this.entitySubscriptions.push(valueChangesSubscription);
  }

  public initializeTitleSelector() {
    const endUserTitlesSubscription = this.api.plans
      .getEndUserTitles()
      .subscribe((titles: Title[]) => {
        this.endUserTitles = titles.map((title: Title) => {
          return {
            key: title.id,
            display: title.titleName
          };
        });
        if (this.endUser) {
          this.patchForm();
        }
      });
    this.entitySubscriptions.push(endUserTitlesSubscription);
  }

  public selectCountryOptionFunction(event) {
    this.selectedCountry = event.option.value;
    this.entityForm.patchValue({
      country: event.option.value,
    });
  }

  public getPostcode() {
    return this.entityForm.get("postcode").value;
  }

  public getCountrySelectorDefault() {
    this.selectedCountry = environment.defaultCountrySelector;
    if (this.selectedCountry) {
      this.displayDefault = this.countriesSelectionOptions.find((selectedOption) => {
        return selectedOption.key === this.selectedCountry;
      }).display
    }
  }
}
