import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Validators } from '@angular/forms';

import { ApiService } from '../../../../core/api/api.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormComponent } from '../../../base-classes/form-component';
import { Version } from '../../../../shared/models/version';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'tdp-edit-version-notes',
  templateUrl: 'edit-version-notes.component.html',
  styleUrls: ['edit-version-notes.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EditVersionNotesComponent extends FormComponent implements OnInit {
  public readonly maxVersionNotesLength: number = 500;
  public readonly maxVersionQuoteNumberLength: number = 20;
  public planVersionNumber: number;
  public discardChangesButtonText: string;
  public dialogTitle: string;
  public saveButtonText: string;
  public version: Version;

  constructor(private dialogRef: MatDialogRef<EditVersionNotesComponent>,
    private api: ApiService,
    private communication: CommunicationService,
    @Inject(MAT_DIALOG_DATA) public data: { versionId: number }
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
    this.version.versionNotes = this.entityForm.get('versionNotes').value;
    this.version.quoteOrderNumber = this.entityForm.get('quoteOrderNumber').value;
    const subscription = this.api.plans.modifyVersionNotesAndQuote(
      this.data.versionId,
      this.version.versionNotes,
      this.version.quoteOrderNumber
    ).pipe(finalize(() => {
      this.dialogRef.close();
    }))
      .subscribe(() => {
        this.communication.notifyReloadViewData();
      });
    this.entitySubscriptions.push(subscription);
  }

  private getVersionNotes(): void {
    const versionSubscription = this.api.plans.getVersionById(this.data.versionId)
      .subscribe((versionModel: Version) => {
        this.entityForm.get('versionNotes').setValue(versionModel.versionNotes);
        this.entityForm.get('quoteOrderNumber').setValue(versionModel.quoteOrderNumber);
        this.version = versionModel;
      });
    this.entitySubscriptions.push(versionSubscription);
  }

}
