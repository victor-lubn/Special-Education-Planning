import { FormComponent } from '../../../../base-classes/form-component';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { User } from '../../../../models/user';
import { Aiep } from '../../../../models/Aiep.model';
import { ApiService } from '../../../../../core/api/api.service';
import { Validators } from '@angular/forms';
import { UserInfoService } from '../../../../../core/services/user-info/user-info.service';
import { zip } from 'rxjs';
import { map } from 'rxjs/operators';
import { Role } from '../../../../models/role';
import { SelectOptionInterface } from '../../../atoms/select/select.component';
import { ValueContainedIn } from 'src/app/shared/validators/value-contained.validator';

@Component({
  selector: 'tdp-user-create-and-details-form',
  templateUrl: 'user-create-and-details-form.component.html',
  styleUrls: ['user-create-and-details-form.component.scss']
})
export class UserCreateAndDetailsFormComponent extends FormComponent implements OnInit, OnChanges {
  public AiepList: Aiep[];
  public AiepCodeList: string[];
  public roleList: SelectOptionInterface[];
  public hasRolePermission: boolean;
  public isLeaver: boolean;

  @Input()
  public user: User;

  @Output()
  public userSubmitted = new EventEmitter<User>();

  constructor(
    private api: ApiService,
    private userInfo: UserInfoService
  ) {
    super();
  this.AiepList = [];
  this.AiepCodeList = [];
  this.roleList = [];
  this.hasRolePermission = false;
  }

  ngOnInit(): void {
    this.hasRolePermission = this.userInfo.hasPermission('Role_Management');
    this.initializeForm(this.user);
    if (!this.hasRolePermission) {
      const roleIdControl = this.entityForm.get('roleId');
      roleIdControl.setValidators(null);
      this.getAieps();
    } else {
      this.getRolesAndAieps();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.user) {
      this.initializeForm(changes.user.currentValue);
      if (changes.user.currentValue) {
        this.patchForm();
      }
    }
  }

  public submitForm(): void {
    if (!this.entityForm.valid) {
      this.entityForm.markAllAsTouched();
      return;
    }
      this.userSubmitted.emit({
      ...this.entityForm.value,
      AiepId: this.AiepList.find(option => option.AiepCode === this.entityForm.controls['AiepCode'].value).id,
      id: this.user ? this.user.id : 0,
      leaver: false
    });
  }

  public getRolesAndAieps(): void {
    const zipSubscription = zip(
      this.api.roles.getAllRoles(),
      this.api.Aieps.getAllAieps()
    ).pipe(
      map(this.mapZipResults)
    ).subscribe((zipResult) => {
      this.initializeRole(zipResult.roleList);
      this.initializeAiep(zipResult.AiepList);
      if (this.user) {
        this.patchForm();
      }
    });
    this.entitySubscriptions.push(zipSubscription);
  }

  private getAieps(): void {
    const subscription = this.api.Aieps.getAllAieps().subscribe((data) => {
      this.initializeAiep(data);
      if (this.user) {
        this.patchForm();
      }
    });
    this.entitySubscriptions.push(subscription);
  }
  
  private initializeAiep(AiepList: Aiep[]) {
    this.AiepList = AiepList;
    this.initializeAiepCodeAutocomplete();
  }

  private initializeAiepCodeAutocomplete() {
    this.AiepCodeList = this.AiepList?.map(Aiep => Aiep.AiepCode);
    this.addValidators(
      this.entityForm.controls['AiepCode'],
      [ValueContainedIn<string>(this.AiepCodeList), Validators.required]
    );
  }

  private initializeRole(roleList: Role[]) {
    this.roleList = roleList.map((roleItem: Role) => {
      return {
        value: roleItem.id,
        text: roleItem.name
      };
    });
  }

  private mapZipResults(apiData) {
    return {
      roleList: apiData[0],
      AiepList: apiData[1]
    };
  }

  private patchForm() {
    if (this.AiepList) {
      const Aiep = this.AiepList.find(option => option.id === this.user.AiepId);
      this.entityForm.patchValue({
        ...this.user,
        AiepCode: Aiep ? Aiep.AiepCode : null,
        AiepId: Aiep ? Aiep.id : null
      });
      this.user.AiepCode = Aiep ? Aiep.AiepCode : null;
    }
  }

  private initializeForm(user: User): void {
    this.entityForm = this.formBuilder.group({
      firstName: [user?.firstName, Validators.required],
      surname: [user?.surname],
      uniqueIdentifier: [user?.uniqueIdentifier, Validators.required],
      roleId: [user?.roleId, Validators.required],
      AiepCode: [user?.AiepCode, Validators.required],
      leaver: [null],
      delegateToUserId: [null]
    });
  }

  //overriding Cancel method as unable to reset 'Delegate To' field visibility
  public cancelEdit(originalEntityValue: any): void {
    this.editing = false;
    if (originalEntityValue) {
      this.entityForm.patchValue(originalEntityValue);
      this.isLeaver = originalEntityValue.leaver;
      this.entityForm.markAsPristine();
    } else {

      this.entityForm.reset();
    }
    this.entityForm.disable();
  }
}

