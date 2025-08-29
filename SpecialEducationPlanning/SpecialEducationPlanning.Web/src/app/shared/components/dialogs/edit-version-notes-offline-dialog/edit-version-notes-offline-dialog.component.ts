import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Validators, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { TranslateService } from '@ngx-translate/core';

import { FormComponent } from '../../../base-classes/form-component';
import { VersionOffline } from '../../../models/plan-offline';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { CommunicationService } from '../../../../core/services/communication/communication.service';

@Component({
  selector: 'tdp-edit-version-notes',
  templateUrl: 'edit-version-notes-offline-dialog.component.html',
  styleUrls: ['edit-version-notes-offline-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EditVersionNotesOfflineComponent extends FormComponent implements OnInit {
  public editNotesForm: UntypedFormGroup;
  public version: VersionOffline;

  public readonly maxVersionNotesLength: number = 500;
  public readonly maxVersionQuoteNumberLength: number = 20;

  constructor(private dialogRef: MatDialogRef<EditVersionNotesOfflineComponent>,
    private offlineMiddleware: OfflineMiddlewareService,
    private translate: TranslateService,
    private communication: CommunicationService,
    @Inject(MAT_DIALOG_DATA) public data: { version: VersionOffline, planId: number }
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      versionNotes: ['', Validators.maxLength(this.maxVersionNotesLength)],
      quoteOrderNumber: ['', Validators.maxLength(this.maxVersionQuoteNumberLength)]
    });
  }

  ngOnInit(): void {

    this.getVersionNotes();
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public saveVersionNote(): void {
    const versionNotesEditModel = {
      planId: this.data.planId,
      versionId: this.data.version.id_offline,
      versionNotes: this.entityForm.get('versionNotes').value,
      quoteOrderNumber: this.entityForm.get('quoteOrderNumber').value
    };
    const editVersionNotesSubscription = this.offlineMiddleware.editVersionNotesObservable(versionNotesEditModel)
      .subscribe((version: VersionOffline) => {
        this.version = version;
        this.communication.notifyReloadViewData();
        this.closeDialog();
      });
    this.entitySubscriptions.push(editVersionNotesSubscription);
  }

  /**
   * Display into dialog actual Version Notes
   */
  private getVersionNotes(): void {
    this.entityForm.get('versionNotes').setValue(this.data.version.versionNotes);
    this.entityForm.get('quoteOrderNumber').setValue(this.data.version.quoteOrderNumber);
  }

}
