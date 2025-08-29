import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Validators } from '@angular/forms';
import { zip } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from 'src/app/core/api/api.service';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { Area } from 'src/app/shared/models/area';
import { Aiep } from 'src/app/shared/models/Aiep.model';
import { User } from 'src/app/shared/models/user';
import { getPostcodeValidator } from '../../../../validators/control-validators/postcode';

@Component({
  selector: 'tdp-Aiep-details-form',
  templateUrl: './Aiep-details-form.component.html',
  styleUrls: ['./Aiep-details-form.component.scss']
})
export class AiepDetailsFormComponent extends FormComponent implements OnInit, OnChanges {

  @Input()
  Aiep: Aiep;

  @Output()
  AiepSubmitted = new EventEmitter<Aiep>();

  public areaNameList: string[];
  public managerUsernameList: string[];

  private areaList: Area[];
  private managerList: User[];

  protected readonly ipAddressRegex = '^(([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\.){3}([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])$';

  constructor(
    private api: ApiService
  ) {
    super();
    this.areaNameList = [];
    this.managerUsernameList = [];
    this.areaList = [];
    this.managerList = [];
   }

  ngOnInit(): void {
    this.initializeForm(this.Aiep);
    this.initializeLists();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.Aiep) {
      this.initializeForm(changes.Aiep.currentValue);
      this.patchForm();
    }
  }

  public displayWith(value: string) {
    return value || '';
  }

  public submitForm(): void {
    if (!this.entityForm.valid) {
      this.entityForm.markAllAsTouched();
      return;
    }

    const manager = this.entityForm.controls['managerName'].value;
    this.AiepSubmitted.emit({
      ...this.entityForm.value,
      downloadLimit: +this.entityForm.controls['downloadLimit'].value,
      downloadSpeed: +this.entityForm.controls['downloadSpeed'].value,
      areaId: this.areaList.find(option => option.keyName === this.entityForm.controls['areaName'].value).id,
      managerId: manager ? this.managerList.find(option => option.uniqueIdentifier === manager).id : null
    });
  }

  private initializeForm(Aiep: Aiep): void {
    this.entityForm = this.formBuilder.group({
      AiepCode: [Aiep?.AiepCode, Validators.required],
      name: [Aiep?.name, Validators.required],
      email: [Aiep?.email, [Validators.required, Validators.email]],
      postcode: [Aiep?.postcode, [Validators.required, this.getPostcodeValidation()]],
      address1: [Aiep?.address1, Validators.required],
      address2: [Aiep?.address2],
      address3: [Aiep?.address3],
      faxNumber: [Aiep?.faxNumber],
      phoneNumber: [Aiep?.phoneNumber],
      areaName: [Aiep?.area?.keyName, Validators.required],
      managerName: [Aiep?.managerId],
      ipAddress: [Aiep?.ipAddress, [Validators.pattern(this.ipAddressRegex)]],
      downloadLimit: [Aiep?.downloadLimit, [Validators.min(0), Validators.max(100), Validators.required]],
      downloadSpeed: [Aiep?.downloadSpeed, [Validators.min(0), Validators.required]],
      htmlEmail: [Aiep?.htmlEmail || false]
    });
  }

  private initializeLists(): void {
    const zipSubscription = zip(
      this.api.areas.getAllAreas(),
      this.api.users.getAllUsersWithRoles()
    ).pipe(
      map(this.mapZipResults)
    ).subscribe((zipResult) => {
      this.initializeAreas(zipResult.areaListResponse);
      this.initializeManagers(zipResult.userListResponse);
      this.patchForm();
    });
    this.entitySubscriptions.push(zipSubscription);
  }

  private mapZipResults(apiData) {
    return {
      areaListResponse: apiData[0],
      userListResponse: apiData[1]
    };
  }

  private initializeAreas(areaList: Area[]): void {
    this.areaList = areaList;
    this.areaNameList = this.areaList.map(area => area.keyName);
  }

  private initializeManagers(managerList: User[]): void {
    this.managerList = managerList;
    this.managerUsernameList = this.managerList.map(user => user.uniqueIdentifier);
  }

  private patchForm(): void {
    const manager = this.managerList.find(managerItem => managerItem.id === this.Aiep?.managerId);
    const area = this.areaList.find(areaItem => areaItem.id === this.Aiep?.areaId);
    this.entityForm.patchValue({
      ...this.Aiep,
      managerName: manager ? manager.uniqueIdentifier : null,
      areaName: area ? area.keyName : null
    });
  }

  private getPostcodeValidation() {
    return getPostcodeValidator().getValidator;
  }
}

