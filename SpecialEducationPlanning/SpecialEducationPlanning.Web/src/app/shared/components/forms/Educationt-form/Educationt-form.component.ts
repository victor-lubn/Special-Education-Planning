import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { UntypedFormGroup, Validators } from '@angular/forms';

import { zip } from 'rxjs';
import { map } from 'rxjs/operators';

import { Aiep } from '../../../models/Aiep.model';
import { FormComponent } from '../../../base-classes/form-component';
import { Address } from '../../../models/address';
import { Area } from '../../../models/area';
import { ApiService } from '../../../../core/api/api.service';
import { ValueContainedIn } from '../../../validators/value-contained.validator';
import { User } from '../../../models/user';
import { COUNTRY_SELECTOR_OPTIONS } from '../../../models/country-selector-options';
import { environment } from './../../../../../environments/environment';

@Component({
  selector: 'tdp-Aiep-form',
  templateUrl: 'Aiep-form.component.html',
  styleUrls: ['Aiep-form.component.scss']
})
export class AiepFormComponent extends FormComponent implements OnInit {

  public countriesSelectionOptions: any;
  public selectedCountry: string;
  public displayDefault?: any;
  
  public areaNameList: string[];
  public managerUsernameList: string[];

  private areaList: Area[];
  private managerList: User[];

  @Input()
  public AiepValidation: UntypedFormGroup;
  
  @Input()
  public Aiep: Aiep;

  @Input()
  public showCountrySelector: boolean;

  @Output()
  public formHasChanges = new EventEmitter<boolean>();

  @Output()
  public AiepSubmitted = new EventEmitter<Aiep>();

  constructor(
    private api: ApiService
  ) {
    super();
    this.areaList = [];
    this.areaNameList = [];
    this.managerList = [];
    this.managerUsernameList = [];
    this.countriesSelectionOptions = COUNTRY_SELECTOR_OPTIONS;
  }

  ngOnInit(): void {
    this.entityForm = this.AiepValidation;
    this.initializeAiepFormControl();
    this.getCountrySelectorDefault();
    const zipSubscription = zip(
      this.api.areas.getAllAreas(),
      this.api.users.getAllUsersWithRoles()
    ).pipe(
      map(this.mapZipResults)
    ).subscribe((zipResult) => {
      this.initializeAreaAutocomplete(zipResult.areaListResponse);
      this.initializeManagerAutocomplete(zipResult.userListResponse);
      // Aiep details
      if (this.Aiep) {
        this.patchForm();
      }
    });
    this.entitySubscriptions.push(zipSubscription);
  }

  protected initializeAiepFormControl() {
    const valueChangesSubscription = this.entityForm.valueChanges.subscribe(() => {
      this.formHasChanges.emit(this.entityForm.dirty);
    });
    this.entitySubscriptions.push(valueChangesSubscription);
  }

  private initializeAreaAutocomplete(areaList: Area[]): void {
    this.areaList = areaList;
    this.areaNameList = this.areaList.map(area => area.keyName);
    this.addValidators(
      this.entityForm.controls['areaName'],
      [ValueContainedIn<string>(this.areaNameList), Validators.required]
    );
  }

  private initializeManagerAutocomplete(managerList: User[]): void {
    this.managerList = managerList;
    this.managerUsernameList = this.managerList.map(user => user.uniqueIdentifier);
    this.addValidators(
      this.entityForm.controls['managerName'],
      ValueContainedIn<string>(this.managerUsernameList)
    );
  }

  private mapZipResults(apiData) {
    return {
      areaListResponse: apiData[0],
      userListResponse: apiData[1]
    };
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
    const manager = this.entityForm.controls['managerName'].value;
    this.AiepSubmitted.emit({
      ...this.entityForm.value,
      areaId: this.areaList.find(option => option.keyName === this.entityForm.controls['areaName'].value).id,
      managerId: manager ? this.managerList.find(option => option.uniqueIdentifier === manager).id : null
    });
  }

  private patchForm(): void {
    const manager = this.managerList.find(managerItem => managerItem.id === this.Aiep.managerId);
    const area = this.areaList.find(areaItem => areaItem.id === this.Aiep.areaId);
    this.entityForm.patchValue({
      ...this.Aiep,
      managerName: manager ? manager.uniqueIdentifier : null,
      areaName: area ? area.keyName : null
    });
    this.entityForm.disable();
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

