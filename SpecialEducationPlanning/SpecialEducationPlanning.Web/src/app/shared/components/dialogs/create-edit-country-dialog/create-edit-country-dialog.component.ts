import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, Validators } from '@angular/forms';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { ApiService } from '../../../../core/api/api.service';
import { FormComponent } from '../../../base-classes/form-component';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { Country } from '../../../models/country.model';

@Component({
  selector: 'tdp-create-edit-country-dialog',
  templateUrl: 'create-edit-country-dialog.component.html',
  styleUrls: ['create-edit-country-dialog.component.scss']
})
export class CreateEditCountryDialogComponent extends FormComponent implements OnInit {

  public country: Country;
  public keyNameString: Country;

  constructor(
    private notifications: NotificationsService,
    private communication: CommunicationService,
    private api: ApiService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<CreateEditCountryDialogComponent>,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) private inputData: { country: Country }
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      keyName: [null, [Validators.required]]
    });
    this.dialogRef.disableClose = true;
    this.country = this.inputData.country;
  }

  ngOnInit(): void {
    if (this.country) {
      this.entityForm.patchValue({
        keyName: this.country.keyName
      });
    }
    const subscription = this.translate.get([
      'notification.countryCreatedSuccess',
      'notification.countryCreatedError',
      'notification.countryEditedSuccess',
      'notification.countryEditedError',
      'dialog.createEditCountry.name'
    ]).subscribe((translations) => {
      this.translations = translations;
      this.keyNameString = translations['dialog.createEditCountry.name']
    });
    this.entitySubscriptions.push(subscription);
  }

  public cancelDialog(): void {
    if (this.entityForm.dirty) {
      const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent,
        {
          width: '400px',
          data: {
            titleStringKey: 'dialog.createEditCountry.titleCreate',
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

  public createCountry(): void {
    const subscription = this.api.countries.createCountry({ ...this.entityForm.value })
      .subscribe(
        this.successResponseHandler.bind(this, 'notification.countryCreatedSuccess'),
        this.errorResponseHandler.bind(this, 'notification.countryCreatedError')
      );
    this.entitySubscriptions.push(subscription);
  }

  public editCountry(): void {
    const countryObject: Country = {
      ...this.entityForm.value,
      id: this.country.id,
    };
    const subscription = this.api.countries.updateCountry(this.country.id,
      countryObject)
      .subscribe(
        this.successResponseHandler.bind(this, 'notification.countryEditedSuccess'),
        this.errorResponseHandler.bind(this, 'notification.countryEditedError')
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
