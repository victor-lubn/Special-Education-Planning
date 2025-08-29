import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Validators } from '@angular/forms';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

import { Permission } from '../../../models/permission.model';
import { FormComponent } from '../../../base-classes/form-component';
import { ApiService } from '../../../../core/api/api.service';
import { Role } from '../../../models/role';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { AssignEntitiesChangeEvent } from '../../assign-entities/assign-entities.component';
import { CommunicationService } from '../../../../core/services/communication/communication.service';

@Component({
  selector: 'tdp-assign-permissions-dialog',
  templateUrl: 'assign-permissions-dialog.component.html',
  styleUrls: ['assign-permissions-dialog.component.scss']
})
export class AssignPermissionsDialogComponent extends FormComponent implements OnInit {
  role: Role;

  public permissionsAssigned: Permission[];
  public permissionsAvailable: Permission[];
  public permissionDisplayProperty: string;
  public listsTitleKey: string;
  public leftInputTitle: string;
  public rightInputTitle: string;
  public permissionsChanged: boolean;

  constructor(
    private api: ApiService,
    private communication: CommunicationService,
    private notifications: NotificationsService,
    private translate: TranslateService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<AssignPermissionsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { displayProperty: string, role: Role, listsTitleKey: string }
  ) {
    super();
    this.dialogRef.disableClose = true;
    this.entityForm = this.formBuilder.group({
      id: [null],
      name: [null, Validators.required]
    });
    this.listsTitleKey = this.data.listsTitleKey;
    this.permissionsAssigned = [];
    this.permissionsAvailable = [];
    this.permissionsChanged = false;
    this.permissionDisplayProperty = this.data.displayProperty;
  }

  ngOnInit(): void {
    let rolesSubscription: Subscription;
    if (this.data.role) {
      this.role = this.data.role
      this.entityForm.patchValue(this.data.role);
      rolesSubscription = this.api.roles.getAssignedAvailablePermissionsByRole(this.data.role.id)
        .subscribe((response) => {

          this.permissionsAssigned = response.permissionAssigned;
          this.permissionsAvailable = response.permissionsAvailable;
        });
      this.entitySubscriptions.push(rolesSubscription);
    } else {
      rolesSubscription = this.api.roles.getAllPermissions()
        .subscribe((response) => {
          this.permissionsAvailable = response;
        });
      this.entitySubscriptions.push(rolesSubscription);
    }
    const translateSubscription = this.translate.get([
      'dialog.newRoleSuccess',
      'dialog.newRoleError',
      'dialog.updateRoleSuccess',
      'dialog.updateRoleError',
      this.data.listsTitleKey,
      'assignEntities.assignedListTitle',
      'assignEntities.availableListTitle'
    ]).subscribe((translations) => {
      this.translations = translations;
      this.leftInputTitle = `${translations[this.data.listsTitleKey]} ${translations['assignEntities.assignedListTitle']}`
      this.rightInputTitle = `${translations[this.data.listsTitleKey]} ${translations['assignEntities.availableListTitle']}`
    });
    this.entitySubscriptions.push(translateSubscription);
  }

  public closeDialog(): void {
    if (this.permissionsChanged || this.entityForm.dirty) {
      const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
        width: '400px',
        data: {
          titleStringKey: 'dialog.manageRoleTitle',
          messageStringKey: 'dialog.genericCancelDialog'
        }
      });
      const dialogSubscription = dialogRef.afterClosed()
        .subscribe((confirmation: boolean) => {
          if (confirmation) {
            this.dialogRef.close();
          }
        });
      this.entitySubscriptions.push(dialogSubscription);
    } else {
      this.dialogRef.close();
    }
  }

  public onChangedLists(listsChange: AssignEntitiesChangeEvent<Permission>) {
    this.permissionsAssigned = listsChange.assigned;
    this.permissionsAvailable = listsChange.available;
    this.permissionsChanged = true;
  }

  public getDisabledState(): boolean {
    return this.data.role ?
      !(this.permissionsChanged || this.entityForm.dirty) || !this.entityForm.valid :
      !(this.permissionsChanged && this.entityForm.dirty) || !this.entityForm.valid;
  }

  public submitRoleWithPermissions(): void {
    if (this.data.role) {
      const subscription = this.api.roles.updateRoleWithPermissions(
        { id: this.data.role.id, name: this.entityForm.controls['name'].value },
        this.permissionsAssigned.map(item => item.id)
      )
        .subscribe(
          this.subscribeSuccessHandler.bind(this, 'dialog.updateRoleSuccess'),
          this.subscribeErrorHandler.bind(this, 'dialog.updateRoleError')
        );
      this.entitySubscriptions.push(subscription);
    } else {
      const subscription = this.api.roles.createRoleWithPermissions(
        this.entityForm.controls['name'].value,
        this.permissionsAssigned.map(item => item.id)
      )
        .subscribe(
          this.subscribeSuccessHandler.bind(this, 'dialog.newRoleSuccess'),
          this.subscribeErrorHandler.bind(this, 'dialog.newRoleError')
        );
      this.entitySubscriptions.push(subscription);
    }
  }

  private subscribeSuccessHandler(translateKey: string): void {
    this.notifications.success(this.translations[translateKey]);
    this.communication.notifyReloadViewData();
    this.dialogRef.close();
  }

  private subscribeErrorHandler(translateKey: string): void {
    this.notifications.error(this.translations[translateKey]);
  }

  closeModal() {
    this.dialogRef.close();
  }
}
