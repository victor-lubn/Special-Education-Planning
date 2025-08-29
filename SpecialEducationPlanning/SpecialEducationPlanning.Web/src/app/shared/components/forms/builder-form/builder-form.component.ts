import { Component, Input, EventEmitter, Output, OnInit } from '@angular/core';
import { Builder } from '../../../models/builder';
import { FormComponent } from '../../../base-classes/form-component';
import { Address } from '../../../models/address';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { UntypedFormGroup } from '@angular/forms';
import { COUNTRY_SELECTOR_OPTIONS } from '../../../models/country-selector-options';
import { environment } from './../../../../../environments/environment';

@Component({
  selector: 'tdp-builder-form',
  templateUrl: 'builder-form.component.html',
  styleUrls: ['builder-form.component.scss']
})
export class BuilderFormComponent extends FormComponent implements OnInit {

  public mandatoryFieldsModified: boolean;
  private readonly mandatoryFieldsList: string[] = ['tradingName', 'postcode', 'address1'];

  public countriesSelectionOptions: any;
  public selectedCountry: string;
  public displayDefault?: any;

  @Input()
  public validation: UntypedFormGroup;

  @Input()
  public builder: Builder;

  @Input()
  public hideEditButtons: boolean;

  @Input()
  public showCountrySelector: boolean;

  @Output()
  public formHasChanges = new EventEmitter<boolean>();

  @Output()
  public builderSubmitted = new EventEmitter<Builder>();

  @Output()
  public builderAssignPlanSubmitted = new EventEmitter<Builder>();

  @Output()
  public returnOrCancelEmitted = new EventEmitter<void>();

  constructor(
    private dialogs: DialogsService
  ) {
    super();
    this.mandatoryFieldsModified = false;
    this.countriesSelectionOptions = COUNTRY_SELECTOR_OPTIONS;
  }

  ngOnInit(): void {
    this.entityForm = this.validation;
    this.initializeBuilderFormControl();
      // Builder details
      if (this.builder) {
        this.patchForm(this.builder);
      }
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

  public enableEdit(): void {
    if (this.builder.accountNumber) {
      this.editing = true;
      this.entityForm.controls['notes'].enable();
    } else {
      super.enableEdit();
    }
  }

  public submitCancel(): void {
    this.returnOrCancelEmitted.emit();
  }

  public submitForm(): void {
    this.mandatoryFieldsModified = this.areMandatoryFieldsModified(this.mandatoryFieldsList);
    const submittedBuilder = this.transformBuilder(this.entityForm.getRawValue());
    this.builderSubmitted.emit(submittedBuilder);
  }

  public submitFormAssign(): void {
    const submmitedBuilder = this.transformBuilder(this.entityForm.getRawValue());
    this.builderAssignPlanSubmitted.emit(submmitedBuilder);
  }

  private transformBuilder(submittedBuilder: Builder): Builder {
    const newMobileNumber = this.entityForm.get('mobileNumber').value;
    const newLandLineNumber = this.entityForm.controls['landLineNumber'].value;
    const newsAPAccountStatus = this.entityForm.controls['sAPAccountStatus'].value;
    return {
      ...submittedBuilder,
      landLineNumber: newLandLineNumber ? newLandLineNumber.replace(' ', '') : null,
      mobileNumber: newMobileNumber ? newMobileNumber.replace(' ', '') : null,
      sapAccountStatus: newsAPAccountStatus ? newsAPAccountStatus.replace(' ', '') : null
    };
  }

  public patchForm(builder: Builder) {
    this.entityForm.patchValue(builder);
    this.entityForm.disable();
  }

  private initializeBuilderFormControl() {
    const valueChangesSubscription = this.entityForm.valueChanges.subscribe(() => {
      this.formHasChanges.emit(this.entityForm.dirty);
    });
    this.entitySubscriptions.push(valueChangesSubscription);
  }

  private areMandatoryFieldsModified(fieldList: string[]): boolean {
    return this.builder && fieldList.some(fieldName => this.builder[fieldName] !== this.entityForm.controls[fieldName].value);
  }

  public selectCountryOptionFunction(event) {
    this.selectedCountry = event.option.value;
    this.entityForm.patchValue({
      country: event.option.value,
    })
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
