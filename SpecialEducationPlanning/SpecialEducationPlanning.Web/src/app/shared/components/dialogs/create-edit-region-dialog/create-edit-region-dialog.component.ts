import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, Validators } from '@angular/forms';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { ApiService } from '../../../../core/api/api.service';
import { FormComponent } from '../../../base-classes/form-component';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { Region } from '../../../models/region';

@Component({
  selector: 'tdp-create-edit-region-dialog',
  templateUrl: 'create-edit-region-dialog.component.html',
  styleUrls: ['create-edit-region-dialog.component.scss']
})
export class CreateEditRegionDialogComponent extends FormComponent implements OnInit {

  public region: Region;
  public countryId: number;
  keyNameString: string;

  constructor(
    private notifications: NotificationsService,
    private communication: CommunicationService,
    private api: ApiService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<CreateEditRegionDialogComponent>,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) private inputData: { region: Region, countryId: number }
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      keyName: [null, [Validators.required]]
    });
    this.dialogRef.disableClose = true;
    this.region = this.inputData.region;
    this.countryId = this.inputData.countryId;
  }

  ngOnInit(): void {
    if (this.region) {
      this.entityForm.patchValue({
        keyName: this.region.keyName
      });
    }
    const subscription = this.translate.get([
      'notification.regionCreatedSuccess',
      'notification.regionCreatedError',
      'notification.regionEditedSuccess',
      'notification.regionEditedError',
      'dialog.createEditRegion.name'
    ]).subscribe((translations) => {
      this.translations = translations;
      this.keyNameString = translations['dialog.createEditRegion.name']
    });
    this.entitySubscriptions.push(subscription);
  }

  public cancelDialog(): void {
    if (this.entityForm.dirty) {
      const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent,
        {
          width: '400px',
          data: {
            titleStringKey: 'dialog.createEditRegion.titleCreate',
            messageStringKey: 'dialog.genericCancelDialog'
          }
        });
      const dialogSubscription = confirmationDialogRef.afterClosed()
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

  public createRegion(): void {
    const regionObject: Region = {
      ...this.entityForm.value,
      countryId: this.countryId
    };
    const subscription = this.api.regions.createRegion(regionObject)
      .subscribe(
        this.successResponseHandler.bind(this, 'notification.regionCreatedSuccess'),
        this.errorResponseHandler.bind(this, 'notification.regionCreatedError')
      );
    this.entitySubscriptions.push(subscription);
  }

  public editRegion(): void {
    const regionObject: Region = {
      ...this.entityForm.value,
      id: this.region.id,
      countryId: this.countryId
    };
    const subscription = this.api.regions.updateRegion(this.region.id,
      regionObject)
      .subscribe(
        this.successResponseHandler.bind(this, 'notification.regionEditedSuccess'),
        this.errorResponseHandler.bind(this, 'notification.regionEditedError')
      );
    this.entitySubscriptions.push(subscription);
  }

  private successResponseHandler(translateKey: string): void {
    this.notifications.success(this.translations[translateKey]);
    this.communication.notifyReloadViewData();
    this.dialogRef.close();
  }

  private errorResponseHandler(translateKey: string): void {
    this.notifications.error(this.translations[translateKey]);
  }

  closeModal() {
    this.dialogRef.close();
  }
}
